using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField]
    bool isLocalPlayer = false;

    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
        set { 
            isLocalPlayer = value;
            if (isLocalPlayer)
                DisableVisuals();
        }
    }
    void Start ()
    {
        // Don't need to draw yourself
        if (isLocalPlayer)
            DisableVisuals();
    }

    void Update ()
    {
        if (isLocalPlayer)
        {
            Transform t = Camera.main.transform;
            transform.position = t.position;
            transform.rotation = t.rotation;
        }
    }

    void DisableVisuals()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }
    }
}