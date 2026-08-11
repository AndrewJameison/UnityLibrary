using UnityEngine;

#nullable enable

/*
 * Andrew Jameison
 * 6.30.2026
 * Implemented the interface, and made it a parent to the other interactable objects that were
 * already in the game (Grabbable.cs, Pickup.cs, NPC.cs, etc.)
 * 
 * 07.14.2026
 * IsPaused now prevents the player from interacting with objects while the game is paused, and prevents a new Target from being acquired
 */

/// <summary>
/// Some object in the scene that the player can be interacted with in some way
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// The current highest priority interactable that is accessible by the player. 
    /// </summary>
    public static IInteractable? Target { get; private set; }

    /// <summary>
    /// Decides whether an item can be interacted with
    /// </summary>
    public bool CanInteract { get; set; }

    /// <summary>
    /// How long the player needs to hold down the interaction key to use the object, instant if == 0
    /// </summary>
    public float HoldTime { get; }

    /// <summary>
    /// Played if the player starts interacting with an object with a HoldTime
    /// </summary>
    //public SoundDataSO InitialSFX { get; }

    /// <summary>
    /// Played if CanInteract is false
    /// </summary>
    //public SoundDataSO FailedSFX { get; }

    /// <summary>
    /// Played if the player stops interacting with an object with a HoldTime
    /// </summary>
    //public SoundDataSO CancelSFX { get; }

    /// <summary>
    /// Played when the player interacts with the Target or at the end of an object's HoldTime
    /// </summary>
    //public SoundDataSO SuccessSFX { get; }

    /// <summary>
    /// The public accessor to change the priority Target's interaction values. Called when obj is different than the Target. 
    /// </summary>
    /// <param name="obj">The new IInteractable object under the player's crosshair. Pass in null to empty the target</param>
    public static void SetPriorityTarget(IInteractable obj)
    {
        try
        {
            // Only applies the hover if and when Target and obj are not null, because we reuse this function to empty the target as well
            Target?.RemoveHover();

            obj?.Hover();

            Target = obj;
        }

        // Resets the Target, especially when reloading the scene
        catch (MissingReferenceException) { Target = null; }
    }

    /// <summary>
    /// Searches for the mOutline material attached to every interactable object
    /// </summary>
    /// <param name="obj">The interactable object</param>
    /// <param name="mMat">The outline material if the object has it</param>
    /// <returns>Whether this object has the mOutline material</returns>
    public static bool SetHighlightMat(Renderer rend, out Material? mMat)
    {
        Material[] mList = rend.materials;

        // The outline material can't be referenced directly, we have to search for it and save it at the start
        foreach (Material mat in mList)
        {
            if (mat.name == "mOutline (Instance)")
            {
                mMat = mat;
                return true;
            }
        }

        Debug.LogWarning("Interactable Object: " + rend.gameObject.name + " is missing an outline material!");

        mMat = null;

        return false;
    }

    /// <summary>
    /// Applies the highlight effect when the player gets close enough to the object
    /// </summary>
    public void Highlight();

    /// <summary>
    /// Removes the any highlighted effects as the player is far enough away
    /// </summary>
    public void RemoveHighlight();

    /// <summary>
    /// A special effect that only happens when the player hovers over the interactable object in question
    /// </summary>
    public void Hover();

    /// <summary>
    /// Removes the hover effect 
    /// </summary>
    public void RemoveHover();

    /// <summary>
    /// The effect interacting with this object has on the scene
    /// </summary>
    /// <returns>Whether the interaction was successful</returns>
    public bool Interact();
}
