using HoloToolkit.Unity.InputModule;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ToggleAnimator : MonoBehaviour, IInputClickHandler
{
    [SerializeField] Animator animator;
    PhotonView view;
    
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        view.RPC(nameof(RPCToggle), RpcTarget.All, !animator.enabled);
    }
    
    [PunRPC]
    void RPCToggle(bool isEnabled)
    {
        animator.enabled = isEnabled;
    }
}