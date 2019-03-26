using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class DeployTroops : MonoBehaviour
{
    public Animator animator;
    public Transform sceneRoot;

    void OnEnable()
    {
        animator.SetBool("IsVisible", true);
    }
    
    public void Deploy(GameObject troopToSpawn)
    {
        var troopGameObject = Instantiate(troopToSpawn, sceneRoot);
        troopGameObject.transform.position = transform.position;

        StartCoroutine(HideMenu());
    }

    IEnumerator HideMenu()
    {
        animator.SetBool("IsVisible", false);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
