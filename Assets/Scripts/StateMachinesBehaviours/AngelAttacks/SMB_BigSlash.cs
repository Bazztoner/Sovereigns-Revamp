using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_BigSlash : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //This is received by the SwordScript
        EventManager.DispatchEvent(PlayerColliderEvents.BigSlash, animator.gameObject.name);
    }
}
