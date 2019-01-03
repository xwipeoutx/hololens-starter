using System;
using UnityEngine;
using Vuforia;

enum VuforiaMode
{
    Disabled,
    PlaceThenDisable,
    PlaceAndContinueRunning,
    LetVuforiaHandleIt
}

public class VuforiaManager : MonoBehaviour, ITrackableEventHandler
{
    [SerializeField] Transform worldRoot = null;
    [SerializeField] GameObject instructions = null;

    [Header("Vuforia Mode")] [SerializeField]
    VuforiaMode editorMode = VuforiaMode.Disabled;

    [SerializeField] VuforiaMode hololensMode = VuforiaMode.PlaceThenDisable;
    [SerializeField] VuforiaMode wmrMode = VuforiaMode.Disabled;
    [SerializeField] VuforiaMode androidMode = VuforiaMode.LetVuforiaHandleIt;
    [SerializeField] VuforiaMode iosMode = VuforiaMode.LetVuforiaHandleIt;

    TrackableBehaviour trackable;
    VuforiaMode activeMode;

    void Awake()
    {
#if UNITY_EDITOR
        activeMode = editorMode;
#else
#if UNITY_UWP
    ConfigureUwp();
#endif

#if UNITY_IOS
    activeMode = iosMode;
    ConfigureForHandHeld();
#endif

#if UNITY_ANDROID
    activeMode = androidMode;
    ConfigureForHandHeld();
#endif
#endif

        if (activeMode == VuforiaMode.Disabled)
        {
            DisableVuforia();
        }
        else
        {
            trackable = GetComponent<TrackableBehaviour>();
            if (trackable != null)
                trackable.RegisterTrackableEventHandler(this);
        }
    }

    void ConfigureUwp()
    {
#if UNITY_UWP
        if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
        {
            activeMode = wmrMode;
        }
        else
        {
            activeMode = hololensMode;
            ConfigureForHololens();
        }
#endif
    }

    void DisableVuforia()
    {
        var arCamera = Camera.main.GetComponent<VuforiaBehaviour>();
        if (arCamera != null)
            DestroyImmediate(arCamera);
        
        var errorHandler = Camera.main.GetComponent<DefaultInitializationErrorHandler>();
        if (errorHandler != null)
            DestroyImmediate(errorHandler);
        
        VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = true;
    }

    void ConfigureForHololens()
    {
        VuforiaConfiguration.Instance.DigitalEyewear.EyewearType =
            DigitalEyewearARController.EyewearType.OpticalSeeThrough;
        VuforiaConfiguration.Instance.DigitalEyewear.SeeThroughConfiguration =
            DigitalEyewearARController.SeeThroughConfiguration.HoloLens;
        VuforiaConfiguration.Instance.DeviceTracker.AutoInitAndStartTracker = false;
    }

    void ConfigureForHandHeld()
    {
        VuforiaConfiguration.Instance.DigitalEyewear.EyewearType = DigitalEyewearARController.EyewearType.None;
        VuforiaConfiguration.Instance.DigitalEyewear.SeeThroughConfiguration =
            DigitalEyewearARController.SeeThroughConfiguration.Vuforia;
        VuforiaConfiguration.Instance.DeviceTracker.TrackingMode = DeviceTracker.TRACKING_MODE.POSITIONAL;
        VuforiaConfiguration.Instance.DeviceTracker.AutoInitAndStartTracker = true;
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            OnTrackingLost();
        }
    }

    private void OnTrackingFound()
    {
        TurnOffInstructions();
        
        switch (activeMode)
        {
            case VuforiaMode.PlaceThenDisable:
                SetPosition();
                StopRunning();
                break;
            case VuforiaMode.PlaceAndContinueRunning:
                SetPosition();
                break;
            case VuforiaMode.LetVuforiaHandleIt:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    void SetPosition()
    {
        if (worldRoot != null)
        {
#if WINDOWS_UWP
            if (worldRoot.GetComponent<WorldAnchor>() != null)
            {
                DestroyImmediate(worldRoot.GetComponent<WorldAnchor>());
            }
#endif

            worldRoot.transform.position = transform.position;
            worldRoot.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            worldRoot.gameObject.SetActive(true);

#if WINDOWS_UWP
            worldRoot.gameObject.AddComponent<WorldAnchor>();
#endif
        }
    }

    private void OnTrackingLost()
    {
        TurnOnInstructions();
    }
    
    private void StopRunning()
    {
        Tracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (imageTracker != null)
        {
            imageTracker.Stop();
        }
    }

    private void TurnOnInstructions()
    {
        if (instructions != null)
        {
            instructions.SetActive(true);
        }
    }

    private void TurnOffInstructions()
    {

        if (instructions != null)
        {
            instructions.SetActive(false);
        }
    }

}