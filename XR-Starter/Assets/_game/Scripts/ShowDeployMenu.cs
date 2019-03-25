using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ShowDeployMenu : MonoBehaviour, IInputClickHandler
{
    public GameObject deployMenu;
    public Transform cameraTransform;
    
    public void OnInputClicked(InputClickedEventData eventData)
    {
        var hitPoint = GazeManager.Instance.HitInfo.point;

        deployMenu.transform.position = hitPoint;
        deployMenu.transform.LookAt(cameraTransform, Vector3.up);
        deployMenu.SetActive(true);
    }
}
