﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_GuardBreakAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the PlayerStats.
        animator.SetBool("isBlocking", false);
        EventManager.DispatchEvent(AnimationEvents.SpecialAttack, new object[] { animator.gameObject.name, true });
        EventManager.DispatchEvent(AnimationEvents.GuardBreakAttack, new object[] { animator.gameObject.name });
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the PlayerStats.
        EventManager.DispatchEvent(AnimationEvents.SpecialAttack, new object[] { animator.gameObject.name, false });
    }
}
