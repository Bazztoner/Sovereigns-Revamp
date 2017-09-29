using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_ParryAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the PlayerStats.
        EventManager.DispatchEvent("ParryAttack", new object[] { animator.gameObject.name });
    }
}
