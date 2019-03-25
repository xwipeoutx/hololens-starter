using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployTroops : MonoBehaviour
{
    public Transform sceneRoot;
    
    public void Deploy(GameObject troopToSpawn)
    {
        var troopGameObject = Instantiate(troopToSpawn, sceneRoot);
        troopGameObject.transform.position = transform.position;
        
        gameObject.SetActive(false);
    }
}
