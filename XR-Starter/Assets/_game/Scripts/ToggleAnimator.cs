using ExitGames.Client.Photon;
using HoloToolkit.Unity.InputModule;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ToggleAnimator : MonoBehaviour, IInputClickHandler, IInRoomCallbacks
{
    [SerializeField] Animator animator;
    PhotonView view;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        view = GetComponent<PhotonView>();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        var isEnabled = !animator.enabled;
        SetAnimatorEnabled(isEnabled);
    }

    private void SetAnimatorEnabled(bool isEnabled)
    {
        if (PhotonNetwork.InRoom)
        {
            view.RPC(nameof(RPCSetAnimatorEnabled), RpcTarget.All, isEnabled);
        }
        else
        {
            RPCSetAnimatorEnabled(isEnabled);   
        }
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetAnimatorEnabled(animator.enabled);
        }
    }

    [PunRPC]
    void RPCSetAnimatorEnabled(bool isEnabled)
    {
        animator.enabled = isEnabled;
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}