using UnityEngine;
using Wikitude;

public class ScaleOverride : TransformOverride
{
    public WikitudeCamera wikiCamera;

    public override void DrawableOverride(Trackable trackable, RecognizedTarget target, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        var imageTarget = target as ImageTarget;
        if (imageTarget != null)
        {
            var augmentationToCamera = position - wikiCamera.transform.position;
            float length = augmentationToCamera.magnitude;
            Vector3 direction = augmentationToCamera.normalized;
            position = wikiCamera.transform.position + direction * length * imageTarget.PhysicalTargetHeight;
        }
    }

    public override void CameraOverride(Trackable trackable, Transform camera, RecognizedTarget target, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        var imageTarget = target as ImageTarget;
        if (imageTarget != null)
        {
            Debug.Log("Recognized target: " + imageTarget.Name + " height: " + imageTarget.PhysicalTargetHeight);
        }
    }

    public override void ImagePreviewOverride(Trackable trackable, float imageTargetHeight, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        scale = new Vector3(imageTargetHeight, imageTargetHeight, imageTargetHeight);
    }
}