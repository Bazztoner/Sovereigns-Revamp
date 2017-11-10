using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_Damage : StateMachineBehaviour {

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EventManager.DispatchEvent("DamageExit", new object[] { animator.gameObject.name });
    }
}
