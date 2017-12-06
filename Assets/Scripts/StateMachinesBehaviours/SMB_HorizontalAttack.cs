using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_HorizontalAttack : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the SwordScript
        EventManager.DispatchEvent(AnimationEvents.HorizontalAttack, new object[] { animator.gameObject.name });
    }
}
