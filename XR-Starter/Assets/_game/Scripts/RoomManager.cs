using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class RoomManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks
{
    [SerializeField] int roomId = 0;
    [SerializeField] GameObject spawnPlayerPrefab = null;
    [SerializeField] bool logInfo = true;

    protected virtual void Start()
    {
        if (roomId == 0)
            Debug.LogWarning("Please set your roomId to something other than 0, or you will be syncing with randos");
        
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnValidate()
    {
        // Verify the player prefab is in the correct folder
#if UNITY_EDITOR
        if (spawnPlayerPrefab != null)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(spawnPlayerPrefab);

            if (!System.IO.Path.GetDirectoryName(path).EndsWith("Resources"))
            {
                Debug.LogError("[Sharing] Spawn Player Prefab must be in the root of a 'Resources folder'!",
                    gameObject);
                spawnPlayerPrefab = null;
            }
        }
#endif
    }

    void JoinRoom()
    {
        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = false;
        options.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom("Room" + roomId, options, TypedLobby.Default);
    }

    public void OnConnected()
    {
    }
    
    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnConnectedToMaster()
    {
        if (logInfo) Debug.Log("[Sharing] Connected to Photon, now connecting to room!");
        JoinRoom();
    }

    void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("[Sharing] " + cause);
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public void OnLeftRoom()
    {
    }

    public virtual void OnJoinedRoom()
    {
        if (logInfo) Debug.LogFormat("[Sharing] Successfully connected to room #{0}!", roomId);
        if (spawnPlayerPrefab != null)
        {
            GameObject go = PhotonNetwork.Instantiate(spawnPlayerPrefab.name, Vector3.zero, Quaternion.identity, 0);
            PlayerVisual player = go.GetComponent<PlayerVisual>();
            if (player != null)
                player.IsLocalPlayer = true;
        }
    }
}