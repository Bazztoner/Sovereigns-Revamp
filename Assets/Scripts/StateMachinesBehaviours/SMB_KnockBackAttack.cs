using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_KnockBackAttack : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the PlayerStats.
        EventManager.DispatchEvent(AnimationEvents.KnockBackEnter, new object[] { animator.gameObject.name });
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the PlayerStats
        EventManager.DispatchEvent(AnimationEvents.KnockBackExit, new object[] { animator.gameObject.name });
    }
}
