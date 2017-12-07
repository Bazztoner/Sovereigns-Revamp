using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DoubleEdgedScale : MonoBehaviour
{
    public int damage;
    public float throwForce;

    string _owner;
    public Rigidbody _rb;

    public void Init(Transform parent, string owner)
    {
        transform.parent = parent;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.forward = transform.parent.forward;
        _owner = owner;
    }

    public void Launch()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * throwForce);
        transform.parent = null;
        Destroy(gameObject, 2f);
    }

    public void Launch(Vector3 dir)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.AddForce(dir * throwForce);
        transform.parent = null;
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter(Collider col)
    {
        var check = col.gameObject.layer == Utilities.IntLayers.PLAYERCOLLIDER ||
                    col.gameObject.layer == Utilities.IntLayers.PLAYER ||
                    col.gameObject.layer == LayerMask.NameToLayer("Default");

        if (check)
        {
            var comp = col.GetComponentInParent<PlayerStats>();
            var compName = col.gameObject.name;

            if (compName != _owner)
            {
                if (comp != null) comp.TakeDamage(damage, "Spell", _owner);
                Destroy(gameObject);
            }
        }

    }

}
