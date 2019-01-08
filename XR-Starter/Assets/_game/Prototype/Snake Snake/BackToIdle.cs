using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToIdle : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("JustAte", false);
        animator.SetBool("IsMoving", false);
    }
}
