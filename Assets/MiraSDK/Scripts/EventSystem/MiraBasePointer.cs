using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MiraBasePointer is an abstract class describing what a pointer (ie: a reticle or cursor) should do,
/// this is useful if you want to create your own pointer besides the provided reticle.
///
/// Functions are called from MiraInputModule and help the pointer graphically respond to interaction events.
/// </summary>
public abstract class MiraBasePointer
{
    /// <summary>
    /// This is used by MiraBaseRaycaster to provide raycasts to the input system if it's in Camera mode.
    /// </summary>
    /// <value>The max distance.</value>
    public float maxDistance
    {
        get;
        set;
    }

    // Called on MiraInputModule enable and disable
    public abstract void OnInputModuleEnabled();

    public abstract void OnInputModuleDisabled();

    public abstract void OnPointerEnter(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive);

    public abstract void OnPointerHover(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive);

    public abstract void OnPointerExit(GameObject targetGameObject);

    public abstract void OnPointerClick();

    public abstract void OnPointerClickDown();

    public abstract void OnPointerClickUp();

    /// <summary>
    /// Importantly instances of the pointer must call this on Start to register themselves
    /// </summary>
	public void OnStart(GameObject pointer)
    {
		MiraPointerManager.Instance.OnPointerCreated(pointer, this);
    }
}