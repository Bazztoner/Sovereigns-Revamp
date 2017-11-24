using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerColliders : MonoBehaviour
{
    public List<Collider> allColliders;
    protected Collider _actualCol;

    protected void Start()
    {
        GetColliders();
        AddColliderHandlerEvents();
    }

    protected virtual void GetColliders()
    {

    }

    Transform FindATransform(Transform parent, string name)
    {
        var childs = parent.GetComponentsInChildren<Transform>().Where(x => x != parent);

        foreach (Transform child in childs)
        {
            if (child.name == name) return child;
        }

        return null;
    }

    protected virtual Collider FindCollider(Transform prnt, string colliderName, string parentName)
    {
        var parent = FindATransform(prnt, parentName);

        var col = FindATransform(parent, colliderName);

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

    protected virtual void AddColliderHandlerEvents()
    {
       
    }

    void ColliderActivation(int id)
    {

    }

    protected virtual void ManageColliders(int id)
    {
        _actualCol = allColliders[id];
        var script = _actualCol.GetComponent<SwordScript>();
        EventManager.DispatchEvent("ActivateCollider", script);
        /*_actualCol = allColliders[id];
        _actualCol.enabled = true;
        foreach (var col in allColliders)
        {
            if (col != _actualCol)
            {
                col.enabled = false;
            }
        }*/
    }
}
