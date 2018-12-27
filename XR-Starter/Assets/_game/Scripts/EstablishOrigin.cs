using UnityEngine;
using Vuforia;

public class EstablishOrigin : MonoBehaviour, ITrackableEventHandler
{
    [SerializeField] bool disableVuforiaWhenFound = true;
    [SerializeField] Transform worldRoot = null;

    TrackableBehaviour trackable;

    void Start()
    {
        trackable = GetComponent<TrackableBehaviour>();
        if (trackable != null)
            trackable.RegisterTrackableEventHandler(this);
    }

    void OnDestroy()
    {
        if (trackable != null)
            trackable.UnregisterTrackableEventHandler(this);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
    }

    void OnTrackingFound()
    {
        if (worldRoot != null)
        {
            worldRoot.transform.position = transform.position;
            worldRoot.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        if (disableVuforiaWhenFound)
        {
#if WINDOWS_UWP
			trackable.enabled = false;
			Destroy(trackable);
			VuforiaBehaviour.Instance.enabled = false;
#else
            Tracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            if (imageTracker != null)
                imageTracker.Stop();
#endif
        }
    }

    void OnTrackingLost()
    {
    }
}