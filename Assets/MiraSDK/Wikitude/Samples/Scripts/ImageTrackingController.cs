using UnityEngine;

public class ImageTrackingController : MonoBehaviour
{
    private void Start()
    {
        // When running in the editor, this line adds the TrackerLarge target to the list of currently recognized targets.
        Wikitude.UnityEditorSimulator.AddImageTarget("TrackerLarge", 0, 300.0f);
    }
}