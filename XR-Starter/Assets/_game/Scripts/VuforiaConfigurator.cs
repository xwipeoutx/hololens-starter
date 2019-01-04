using System;
using System.Collections;
using UnityEngine;
using Vuforia;

[Serializable]
public class VuforiaDeviceConfig
{
    public static VuforiaDeviceConfig Disabled => new VuforiaDeviceConfig();
    public static VuforiaDeviceConfig Hololens => new VuforiaDeviceConfig(true, false, true);
    public static VuforiaDeviceConfig HandHeld => new VuforiaDeviceConfig(false, true, false);

    public VuforiaDeviceConfig(bool isTransparent, bool controlsCamera, bool disableOnFind)
    {
        this.isEnabled = true;
        this.isTransparent = isTransparent;
        this.controlsCamera = controlsCamera;
        this.disableOnFind = disableOnFind;
    }

    private VuforiaDeviceConfig()
    {
        this.isEnabled = false;
        this.isTransparent = false;
        this.controlsCamera = false;
        this.disableOnFind = false;
    }

    public bool isEnabled;
    public bool isTransparent;
    public bool controlsCamera;
    public bool disableOnFind;

    public override string ToString()
    {
        if (!isEnabled)
        {
            return "DISABLED";
        }
        else
        {
            return
                $"{(isTransparent ? "See Through" : "Opaque")}, {(controlsCamera ? "Controls Camera" : "Moves Target")}, {(disableOnFind ? "Single Use" : "Stays active")}";
        }
    }
}

public class VuforiaConfigurator : MonoBehaviour, ITrackableEventHandler
{
    [SerializeField] Transform worldRoot = null;
    [SerializeField] GameObject instructions = null;

    [Header("Vuforia Mode")] [SerializeField] [Tooltip("Camera-relative position when disabled")]
    private Vector3 positionWhenDisabled = Vector3.zero;

    [SerializeField] VuforiaDeviceConfig editorConfig = VuforiaDeviceConfig.Disabled;

    [SerializeField] VuforiaDeviceConfig hololensConfig = new VuforiaDeviceConfig(true, false, true);
    [SerializeField] VuforiaDeviceConfig wmrConfig = VuforiaDeviceConfig.Disabled;
    [SerializeField] VuforiaDeviceConfig androidConfig = new VuforiaDeviceConfig(false, true, true);
    [SerializeField] VuforiaDeviceConfig iosConfig = new VuforiaDeviceConfig(false, true, true);

    ImageTargetBehaviour trackable;
    VuforiaDeviceConfig activeConfig;

    private bool isTracked;

    void Awake()
    {
#if UNITY_EDITOR
        activeConfig = editorConfig;
#else
#if WINDOWS_UWP
        activeConfig = UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque ? wmrConfig : hololensConfig;
#endif

#if UNITY_IOS
        activeConfig = iosConfig;
#endif

#if UNITY_ANDROID
        activeConfig = androidConfig;
#endif
#endif

        if (!activeConfig.isEnabled)
        {
            DisableVuforia();
            TurnOffInstructions();
        }
        else
        {
            trackable = GetComponent<ImageTargetBehaviour>();
            if (trackable != null)
                trackable.RegisterTrackableEventHandler(this);

            if (activeConfig.controlsCamera)
            {
                if (trackable == null)
                {
                    VuforiaManager.Instance.WorldCenterMode = VuforiaARController.WorldCenterMode.FIRST_TARGET;
                }
                else
                {
                    VuforiaManager.Instance.WorldCenterMode = VuforiaARController.WorldCenterMode.SPECIFIC_TARGET;
                    VuforiaManager.Instance.WorldCenter = trackable;
                }

                VuforiaManager.Instance.WorldCenterMode = VuforiaARController.WorldCenterMode.SPECIFIC_TARGET;

                VuforiaConfiguration.Instance.DeviceTracker.TrackingMode = DeviceTracker.TRACKING_MODE.POSITIONAL;
                VuforiaConfiguration.Instance.DeviceTracker.AutoInitAndStartTracker = true;
            }
            else
            {
                VuforiaManager.Instance.WorldCenterMode = VuforiaARController.WorldCenterMode.DEVICE;
                VuforiaConfiguration.Instance.DeviceTracker.AutoInitAndStartTracker = false;
            }

            if (activeConfig.isTransparent)
            {
                VuforiaConfiguration.Instance.VideoBackground.VideoBackgroundEnabled = false;

                VuforiaConfiguration.Instance.DigitalEyewear.EyewearType =
                    DigitalEyewearARController.EyewearType.OpticalSeeThrough;
                VuforiaConfiguration.Instance.DigitalEyewear.SeeThroughConfiguration =
                    DigitalEyewearARController.SeeThroughConfiguration.HoloLens;
            }
            else
            {
                VuforiaConfiguration.Instance.VideoBackground.VideoBackgroundEnabled = true;

                VuforiaConfiguration.Instance.DigitalEyewear.EyewearType = DigitalEyewearARController.EyewearType.None;
                VuforiaConfiguration.Instance.DigitalEyewear.SeeThroughConfiguration =
                    DigitalEyewearARController.SeeThroughConfiguration.Vuforia;
            }

            TurnOnInstructions();
        }

        Debug.Log($"Vuforia Config: {activeConfig}");
    }

    void Start()
    {
        // Done in Start because main camera has not yet been put in place at Awake time
        if (!activeConfig.isEnabled)
        {
            var forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();
            var direction = Quaternion.Euler(forward);

            worldRoot.transform.position = Camera.main.transform.position + direction * positionWhenDisabled;
        }
    }

    void OnDestroy()
    {
        if (trackable != null)
            trackable.UnregisterTrackableEventHandler(this);
    }

    void Update()
    {
        if (activeConfig.isEnabled && !activeConfig.disableOnFind && isTracked)
        {
            SetPosition();
        }
    }

    void DisableVuforia()
    {
        DestroyCameraObjects();
        VuforiaConfiguration.Instance.Vuforia.DelayedInitialization = true;
    }

    private static void DestroyCameraObjects()
    {
        VuforiaBehaviour vuforiaBehaviour = Camera.main.GetComponent<VuforiaBehaviour>();
        if (vuforiaBehaviour != null)
            DestroyImmediate(vuforiaBehaviour);

        var errorHandler = Camera.main.GetComponent<DefaultInitializationErrorHandler>();
        if (errorHandler != null)
            DestroyImmediate(errorHandler);
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
        Debug.Log("Target Found");
        TurnOffInstructions();

        isTracked = true;

        if (activeConfig.disableOnFind)
        {
            SetPosition();
            StopRunning();
        }
    }

    void SetPosition()
    {
        Debug.Log("Setting position");
#if WINDOWS_UWP
        if (worldRoot.GetComponent<UnityEngine.XR.WSA.WorldAnchor>() != null)
        {
            DestroyImmediate(worldRoot.GetComponent<UnityEngine.XR.WSA.WorldAnchor>());
        }
#endif

        worldRoot.transform.position = transform.position;
        worldRoot.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

#if WINDOWS_UWP
            worldRoot.gameObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
#endif
    }

    private void OnTrackingLost()
    {
        if (activeConfig.disableOnFind)
            return;

        Debug.Log("Tracking lost");
        TurnOnInstructions();

        isTracked = false;
    }

    private void StopRunning()
    {
        Tracker imageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        if (imageTracker != null)
        {
            imageTracker.Stop();
        }

        StartCoroutine(UnregisterTrackable());
    }

    private IEnumerator UnregisterTrackable()
    {
        yield return null;

        if (trackable != null)
        {
            trackable.UnregisterTrackableEventHandler(this);
            trackable.enabled = false;
            Destroy(trackable);
        }

        VuforiaBehaviour.Instance.enabled = false;
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