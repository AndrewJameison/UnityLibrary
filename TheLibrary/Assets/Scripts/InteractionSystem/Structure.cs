using UnityEngine;
using UnityEngine.Events;
//using AudioSystem;

/*
 * Andrew Jameison
 * 07.01.2026
 * A generic interactable object script for static objects sitting in the scene.
 * This should act as a trigger for functions from other scripts, if this object...
 * ...needs to do anything more, instead create a new child class that inherits IInteractable.
 */

/// <summary>
/// A generic interactable object within the scene. Acts as a trigger for some function elsewhere
/// </summary>
public class Structure : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField]
    private bool _canInteract = true;
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    [SerializeField]
    private float _holdTime = 0.0f;
    public float HoldTime { get => _holdTime; }

    [SerializeField]
    private float _highlightSize = 1.05f;

    // TODO: move all this highlight stuff to its own separate script for cleanliness
    [Tooltip("A function from another object that should be affected when this interactable is clicked on")]
    [SerializeField]
    private UnityEvent _interact;

    [Header("Outline Settings")]
    [SerializeField]
    [Tooltip("The highlight color when the player is in range of the interaction")]
    private Color _highlightNear = Color.yellow;

    [SerializeField]
    [Tooltip("The highlight color when the player is looking at the object")]
    private Color _highlightHover = Color.white;

    /// <summary>
    /// The color applied to this object when in range of the player,
    /// </summary>
    private Color _highlightColor;

    /// <summary>
    /// A reference to the mOutline material which is used by the interaction system to highlight the object
    /// </summary>
    private Material _highlightMat;

    /// <summary>
    /// The primary renderer onto which the mOutline material is attached
    /// </summary>
    [SerializeField]
    private Renderer _rend;

    //#region SFX
    //[Header("Reactions to Interaction SFX")]
    //public SoundDataSO InitialSFX => null;

    //[SerializeField] private SoundDataSO _failedSFX;
    //public SoundDataSO FailedSFX { get => _failedSFX; }

    //public SoundDataSO CancelSFX => null;

    //[SerializeField] private SoundDataSO _successSFX;
    //public SoundDataSO SuccessSFX { get => _successSFX; }
    //#endregion

    void Start()
    {
        // Setup the data for the interaction system 
        _highlightColor = _highlightNear;

        if (_rend != null)
        {
            gameObject.SetActive(IInteractable.SetHighlightMat(_rend, out _highlightMat));
        }
    }

    public void Highlight()
    {
        if (_rend != null)
        {
            _highlightMat.color = _highlightColor;

            _highlightMat.SetFloat("_Thickness", 1.05f);
        }
    }

    public void RemoveHighlight()
    {
        if (_rend != null)
        {
            _highlightMat.SetFloat("_Thickness", 1f);
        }
    }

    public void Hover()
    {
        if (_rend != null)
        {
            _highlightColor = _highlightHover;
        }
    }

    public void RemoveHover()
    {
        if (_rend != null)
        {
            _highlightColor = _highlightNear;
        }
    }

    public bool Interact()
    {
        _interact.Invoke();

        return true;
    }
}
