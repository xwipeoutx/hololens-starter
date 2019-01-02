using Photon.Pun;
using UnityEngine;

namespace Scripts
{
    public class ShareLocalTransform : MonoBehaviour
    {
        [SerializeField] private bool lerp = true;
        [SerializeField] private float checkInterval = 0;
        PhotonView view;
        PlayerVisual playerVisual;

        private bool haveDoneInitialSync;

        private float checkAfter = 0;
        private Vector3 previousPosition;
        private Quaternion previousRotation;

        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private void Awake()
        {
            playerVisual = GetComponent<PlayerVisual>();
            view = GetComponent<PhotonView>();
        }

        void Update()
        {
            if (!playerVisual.IsLocalPlayer)
            {
                if (lerp)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 5 * Time.deltaTime);
                    transform.localRotation =
                        Quaternion.Lerp(transform.localRotation, targetRotation, 5 * Time.deltaTime);
                }

                return;
            }

            if (checkAfter > Time.time || !PhotonNetwork.InRoom)
                return;

            var currentPosition = transform.localPosition;
            var currentRotation = transform.localRotation;

            if (!haveDoneInitialSync || !currentPosition.AlmostEquals(previousPosition, 0.01f) ||
                !currentRotation.AlmostEquals(previousRotation, 0.1f))
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
            view.RPC(nameof(RPCTransformChange), RpcTarget.All, position, rotation);
        }

        [PunRPC]
        void RPCTransformChange(Vector3 position, Quaternion rotation)
        {
            if (lerp)
            {
                targetPosition = previousPosition = position;
                targetRotation = previousRotation = rotation;
            }
            else
            {
                transform.localPosition = targetPosition = previousPosition = position;
                transform.localRotation = targetRotation = previousRotation = rotation;
            }
        }
    }
}