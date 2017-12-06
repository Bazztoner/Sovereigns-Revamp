using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBehaviour : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isRolling", false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (PhotonNetwork.offlineMode)
        {
            EventManager.DispatchEvent(AnimationEvents.RollExit, new object[] { animator.gameObject.name });
        }
        else if (animator.gameObject.GetComponent<PlayerInput>().enabled)
        {
            EventManager.DispatchEvent(AnimationEvents.RollExit, new object[] { animator.gameObject.name });
        }
    }
}
