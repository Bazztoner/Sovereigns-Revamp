using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateVarUnable : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("X", false);
        animator.SetBool("Y", false);
    }

}
