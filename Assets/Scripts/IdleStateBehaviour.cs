using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateBehaviour : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        EventManager.DispatchEvent("IdleEnter");
    }
}
