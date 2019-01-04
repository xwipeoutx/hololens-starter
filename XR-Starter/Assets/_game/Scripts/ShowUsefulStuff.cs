using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ShowUsefulStuff : MonoBehaviour
{
    private Text textMesh;
    [SerializeField] TrackableBehaviour trackableBehaviour;

    private void Awake()
    {
        textMesh = GetComponent<Text>();
    }

    void Update()
    {
        var fusionProvider = VuforiaRuntimeUtilities.GetActiveFusionProvider();
        var trackableStatus = trackableBehaviour.CurrentStatus;
        var tracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        textMesh.text = $"{fusionProvider} | {trackableStatus} | {tracker.IsActive}";
    }
}