using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelAttackColliders : PlayerColliders
{
    enum AttackTypes
    {
        NORMAL_SLASH,
        NORMAL_SHIELD,
        BIG_SLASH,
        BIG_SHIELD,
        Count
    }

    protected override void GetColliders()
    {
        allColliders = new List<Collider>();

        allColliders.Add(FindCollider(transform, "SwordCollider", "Sword"));
        allColliders.Add(FindCollider(transform, "ShieldCollider", "Shield"));
        allColliders.Add(FindCollider(transform, "BigSwordCollider", "Sword"));
        allColliders.Add(FindCollider(transform, "BigShieldCollider", "Shield"));
    }

    protected override void AddColliderHandlerEvents()
    {
        EventManager.AddEventListener("NormalSlash", OnNormalSlash);
        EventManager.AddEventListener("BigSlash", OnBigSlash);
        EventManager.AddEventListener("NormalShield", OnNormalShield);
        EventManager.AddEventListener("BigShield", OnBigShield);
    }

    void OnNormalSlash(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.NORMAL_SLASH;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, false);
                }
            }
        }
    }

    void OnBigSlash(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.BIG_SLASH;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, false);
                }
            }
        }
    }

    void OnNormalShield(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.NORMAL_SHIELD;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, false);
                }
            }
        }
    }

    void OnBigShield(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.name == (string)paramsContainer[0])
            {
                var id = (int)AttackTypes.BIG_SHIELD;
                for (int i = 0; i < allColliders.Count; i++)
                {
                    if (i == id) ManageColliders(id, true);
                    else ManageColliders(id, false);
                }
            }
        }
    }
}
