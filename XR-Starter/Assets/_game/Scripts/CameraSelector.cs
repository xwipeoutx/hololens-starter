using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    void Start()
    {
        var clearFlags = CameraClearFlags.SolidColor;
#if WINDOWS_UWP
        if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
        {
            clearFlags = CameraClearFlags.Skybox;
        }
#endif

#if UNITY_EDITOR
        clearFlags = CameraClearFlags.Skybox;
#endif

        Camera.main.clearFlags = clearFlags;
    }
}