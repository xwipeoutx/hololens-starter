using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;


public class DeployButton : MonoBehaviour, IInputClickHandler
{
    public DeployTroops deployTroops;
    public GameObject troopToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        deployTroops.Deploy(troopToSpawn);
    }
}
