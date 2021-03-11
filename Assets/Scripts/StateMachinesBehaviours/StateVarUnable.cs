using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateVarUnable : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(!animatorStateInfo.IsName("2Block+Y"))
        {
             if(animator.gameObject.GetComponent<PlayerInput>().enabled)
            {
                if (animator.GetBool("X")) EventManager.DispatchEvent(AnimationEvents.AttackEnter, new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().lightAttackDamage });
                else if (animator.GetBool("Y")) EventManager.DispatchEvent(AnimationEvents.AttackEnter, new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().heavyAttackDamage });
            }
        }

        animator.SetBool("X", false);
        animator.SetBool("Y", false);
    }

}
