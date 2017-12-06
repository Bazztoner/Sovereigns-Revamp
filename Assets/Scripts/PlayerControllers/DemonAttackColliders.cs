using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAttackColliders : PlayerColliders
{
    enum AttackTypes
    {
        LEFT_CLAW,
        RIGHT_CLAW,
        DUAL_CLAW,
        Count
    }

    protected override void GetColliders()
    {
        allColliders = new List<Collider>();

        allColliders.Add(FindCollider(transform, "LeftClawCollider", "LeftArm"));
        allColliders.Add(FindCollider(transform, "RightClawCollider", "RightArm"));
        allColliders.Add(FindCollider(transform, "DualClawCollider", "NeckSpine1"));

    }

    protected override void AddColliderHandlerEvents()
    {
        EventManager.AddEventListener(PlayerColliderEvents.LeftClawAttack, OnLeftClawAttack);
        EventManager.AddEventListener(PlayerColliderEvents.RightClawAttack, OnRightClawAttack);
        EventManager.AddEventListener(PlayerColliderEvents.DualClawAttack, OnDualClawAttack);
    }

    void OnLeftClawAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.LEFT_CLAW;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, true);
                }
                
            }
        }
    }

    void OnRightClawAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.RIGHT_CLAW;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, true);
                }
            }
        }
    }

    void OnDualClawAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.DUAL_CLAW;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, true);
                }
            }
        }
    }
}
