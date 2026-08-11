using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Finds objects within range that can be interacted with and marks them as the Target
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private float _interactRange;

    private IEnumerator _co;

    private int _ignorePlayerMask;

    public void Awake()
    {
        _interactRange = GetComponent<SphereCollider>().radius;

        _ignorePlayerMask = ~LayerMask.GetMask("PlayerLayer", "Ignore Raycast");
    }

    public void Update()
    {
        // Exit early while the game is paused, no need to think about interactions
        if (Time.timeScale == 0.0f) return;

        InteractionRoutine();
    }
    /// <summary>
    /// Unity input event to detect the interaction action
    /// </summary>
    /// <param name="context"></param>
    public void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (IInteractable.Target != null)
            {
                if (!IInteractable.Target.CanInteract)
                {
                    //AudioManager.Instance.Play(IInteractable.Target.FailedSFX, transform.position);
                }

                else if (IInteractable.Target.HoldTime == 0.0f)
                {
                    if (IInteractable.Target.Interact())
                    {
                        //AudioManager.Instance.Play(IInteractable.Target.SuccessSFX, transform.position);

                        if (Time.timeScale == 0.0f)
                        {
                            Empty();
                        }
                    }
                    else
                    {
                        //AudioManager.Instance.Play(IInteractable.Target.FailedSFX, transform.position);
                    }
                }

                else
                {
                    //AudioManager.Instance.Play(IInteractable.Target.InitialSFX, transform.position);
                    _co = PressAndHold(IInteractable.Target.HoldTime);
                    StartCoroutine(_co);
                }
            }
        }

    }

    /// <summary>
    /// Unity input event to prevent an interactable from being used when crosshair moves away from Target or lets go of the interaction key
    /// </summary>
    /// <param name="context"></param>
    public void OnInteractCanceled(InputAction.CallbackContext context)
    {
        if (_co != null)
        {
            StopCoroutine(_co);
            _co = null;
        }
    }

    /// <summary>
    /// Cancels any interaction related effects and empties any IInteractable related objects
    /// </summary>
    private void Empty()
    {
        OnInteractCanceled(new InputAction.CallbackContext());
        IInteractable.SetPriorityTarget(null);

        // Disable Interact Icon
    }

    /// <summary>
    /// Called when an interactable obj needs to be held down to interact, only for as long as the player keeps looking at the object
    /// </summary>
    /// <param name="holdTime"></param>
    /// <returns></returns>
    private IEnumerator PressAndHold(float holdTime)
    {
        yield return new WaitForSeconds(holdTime);

        if (IInteractable.Target.Interact())
        {
            //AudioManager.Instance.Play(IInteractable.Target.SuccessSFX, transform.position);

            if (Time.timeScale == 0.0f)
            {
                Empty();
            }
        }
        else
        {
            //AudioManager.Instance.Play(IInteractable.Target.FailureSFX, transform.position);
        }
    }

    /// <summary>
    /// Sends out a single raycast each frame into the scene to select a target for interaction, preventing multiple interactions at a time
    /// </summary>
    private void InteractionRoutine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Only ignore the player's collider when looking for interactions, allowing walls to occlude items
        if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _ignorePlayerMask))
        {
            IInteractable obj = hit.collider.GetComponentInParent<IInteractable>();

            // Looks for only objects IInteractable components somewhere in their hierarchy
            if (obj != null)
            {
                // Do not bother setting a new Target if it is the same focus as before
                if (IInteractable.Target != obj)
                {
                    OnInteractCanceled(new InputAction.CallbackContext());
                    IInteractable.SetPriorityTarget(obj);

                    // Enable Interact Icon
                }

            }

            // We see something in range, but it isn't an interactable object
            else
            {
                Empty();
            }

        }

        // There is nothing in range, so if the target isn't already null, empty it
        else if (IInteractable.Target != null)
        {
            Empty();
        }
    }

    void OnTriggerStay(Collider other)
    {
        IInteractable obj = other.GetComponentInParent<IInteractable>();

        // Only if the object is interactable and is not blocked by another object does it add the highlight
        if (obj != null && obj.CanInteract &&
            Physics.Raycast(transform.position, other.transform.position - transform.position,
            out RaycastHit hit, 10.0f, _ignorePlayerMask))
        {
            // Is there anything between the player and the object?
            if (hit.collider == other)
            {
                obj.Highlight();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        IInteractable obj = other.GetComponentInParent<IInteractable>();

        // Removes the highlight when far enough away
        if (obj != null)
        {
            obj.RemoveHighlight();
        }
    }
}
