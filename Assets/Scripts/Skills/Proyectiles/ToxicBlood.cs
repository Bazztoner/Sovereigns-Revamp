using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ToxicBlood : MonoBehaviour
{
    public int damage;
    public float throwForce;
    public float duration;
    public int tickCoeficient;

    string _owner;
    public Rigidbody _rb;
    LayerMask _layMask;

    void Start()
    {
        damage = 115;
        duration = 3;
        tickCoeficient = 20;

        _layMask = 1 << LayerMask.NameToLayer("Player");
    }

    public void Init(Transform parent, string owner)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.forward = transform.parent.forward;
        _owner = owner;
    }

    public void Launch(Vector3 dir)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.AddForce(dir * throwForce);
    }

    public void Launch(Vector3 dir, float force)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.AddForce(dir * force);
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
                if (comp != null) comp.TakeDamage(damage, "Spell", _owner, duration, tickCoeficient);
                print(col.transform.name);
                Destroy(gameObject);
            }
        }

    }

}
