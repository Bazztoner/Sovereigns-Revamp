using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Colliders : MonoBehaviour
{
    public List<Collider> allColliders;
    Collider _actualCol;

    enum AttackTypes
    {
        NORMAL_SLASH,
        NORMAL_SHIELD,
        BIG_SLASH,
        BIG_SHIELD,
        Count
    }

    void Start()
    {
        GetColliders();
        AddColliderHandlerEvents();
    }

    void GetColliders()
    {
        allColliders = new List<Collider>();

        allColliders.Add(FindCollider(transform, "SwordCollider", "Sword"));
        allColliders.Add(FindCollider(transform, "ShieldCollider", "Shield"));
        allColliders.Add(FindCollider(transform, "BigSwordCollider", "Sword"));
        allColliders.Add(FindCollider(transform, "BigShieldCollider", "Shield"));
    }


    Collider FindCollider(Transform prnt, string colliderName, string parentName)
    {
        var col = prnt.FindChild(colliderName);
        
        if (col == null)
        {
            Transform toEmparent = null;
            var childs = transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in childs)
            {
                if (t.name == parentName)
                {
                    toEmparent = t;
                    break;
                }
            }

            if (toEmparent == null) return null;
            
            var load = GameObject.Instantiate(Resources.Load("New Colliders/" + colliderName) as GameObject, toEmparent, false);
            load.gameObject.name = colliderName;

            load.transform.localPosition = Vector3.zero;

            return load.GetComponent<Collider>();
        }
        else return col.GetComponent<Collider>();
    }

    void AddColliderHandlerEvents()
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
                ManageColliders(id);
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
                ManageColliders(id);
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
                ManageColliders(id);
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
                ManageColliders(id);
            }
        }
    }

    void ManageColliders(int id)
    {
        /*_actualCol = allColliders[id];
        var colName = _actualCol.gameObject.name;
        EventManager.DispatchEvent("ActivateCollider", colName);*/
        _actualCol = allColliders[id];
        _actualCol.enabled = true;
        foreach (var col in allColliders)
        {
            if (col != _actualCol)
            {
                col.enabled = false;
            }
        }
    }
}
