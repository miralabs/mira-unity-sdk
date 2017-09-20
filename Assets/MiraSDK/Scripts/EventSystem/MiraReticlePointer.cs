using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MiraReticlePointer extends MiraBasePointer and calls the scripts within MiraReticle that help it change appearance
/// when the event system processes interaction events
/// </summary>
public class MiraReticlePointer : MiraBasePointer
{
    public override void OnInputModuleEnabled()
    {
    }

    public override void OnInputModuleDisabled()
    {
    }

    public override void OnPointerEnter(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive)
    {
        MiraReticle.Instance.reticleEnter(ray, isTargetInteractive);
    }

    public override void OnPointerHover(GameObject targetGameObject, RaycastResult ray, bool isTargetInteractive)
    {
        MiraReticle.Instance.reticleHover(ray, isTargetInteractive);
    }

    public override void OnPointerExit(GameObject targetGameObject)
    {
        MiraReticle.Instance.reticleExit();
    }

    public override void OnPointerClick()
    {
    }

    public override void OnPointerClickDown()
    {
    }

    public override void OnPointerClickUp()
    {
    }
}