using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_DemonSkills : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("skillScaleLaunch", false);
        animator.SetBool("skillSpit", false);
    }
}
