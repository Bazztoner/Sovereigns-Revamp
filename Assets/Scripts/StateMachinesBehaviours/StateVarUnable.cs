using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateVarUnable : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (PhotonNetwork.offlineMode)
        {
            if (animator.GetBool("X")) EventManager.DispatchEvent("AttackEnter", new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().lightAttackDamage });
            else if (animator.GetBool("Y")) EventManager.DispatchEvent("AttackEnter", new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().heavyAttackDamage });
        }
        else if(animator.gameObject.GetComponent<Player1Input>().enabled)
        {
            if (animator.GetBool("X")) EventManager.DispatchEvent("AttackEnter", new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().lightAttackDamage });
            else if (animator.GetBool("Y")) EventManager.DispatchEvent("AttackEnter", new object[] { animator.gameObject.name, animator.gameObject.GetComponent<PlayerCombat>().heavyAttackDamage });
        }

        animator.SetBool("X", false);
        animator.SetBool("Y", false);
    }

}
