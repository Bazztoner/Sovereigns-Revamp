using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollStateBehaviour : StateMachineBehaviour {

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("isRolling", false);
    }
}
