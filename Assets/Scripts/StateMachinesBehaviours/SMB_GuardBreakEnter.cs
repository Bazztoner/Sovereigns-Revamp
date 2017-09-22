using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_GuardBreakEnter : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("guardBreak", false);
    }
}
