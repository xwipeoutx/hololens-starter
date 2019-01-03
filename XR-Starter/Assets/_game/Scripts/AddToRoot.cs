using UnityEngine;

namespace Scripts
{
    public class AddToRoot : MonoBehaviour
    {
        void Awake()
        {
            var objName = "SceneRoot";
            var root = GameObject.Find(objName);
            if (root == null)
                Debug.LogError("Could not find a " + objName + " object!");
            else
                transform.parent = root.transform;
        }
    }
}