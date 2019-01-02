using Photon.Pun;
using UnityEngine;

namespace Scripts
{
    public class ShareLocalTransform : MonoBehaviour
    {
        [SerializeField] private float checkInterval = 0;
        PhotonView view;
    
        private bool haveDoneInitialSync;

        private float checkAfter = 0;
        private Vector3 previousPosition;
        private Quaternion previousRotation;

        private void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        void Update()
        {
            if (checkAfter > Time.time || !PhotonNetwork.InRoom)
                return;

            var currentPosition = transform.localPosition;
            var currentRotation = transform.localRotation;
        
            if (!haveDoneInitialSync || currentPosition.AlmostEquals(previousPosition, 0.001f) || currentRotation.AlmostEquals(previousRotation, 0.1f))
            {
                checkAfter = Time.time + checkInterval;
                haveDoneInitialSync = true;
                previousPosition = currentPosition;
                previousRotation = currentRotation;
                TransformChange(currentPosition, currentRotation);
            }
        }
    
        void TransformChange(Vector3 position, Quaternion rotation)
        {
            view.RPC(nameof(RPCTransformChange), RpcTarget.Others, position, rotation);
        }
    
        [PunRPC]
        void RPCTransformChange(Vector3 position, Quaternion rotation)
        {
            Debug.Log($"Position now {position}, Rotation now {rotation.eulerAngles}");
            transform.localPosition = previousPosition = position;
            transform.localRotation = previousRotation = rotation;
        }
    }
}