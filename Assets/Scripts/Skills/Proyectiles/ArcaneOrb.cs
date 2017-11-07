using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ArcaneOrb : MonoBehaviour
{
    public int damage;
    public float throwForce;
    public float lifeTime = 3;

    string _owner;
    Rigidbody _rb;
    LayerMask _layMask;

    void Start()
    {
        damage = 135;
        _rb = GetComponent<Rigidbody>();
        _layMask = 1 << LayerMask.NameToLayer("Player");
    }

    public void Init(Transform parent, string owner)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.forward = transform.parent.forward;
        this._owner = owner;
	}

    public void Launch(Vector3 dir)
    {
        _rb.AddForce(dir * throwForce);
        Invoke("StartDeathTimer", lifeTime);
    }

    public void Launch(Vector3 dir, float force)
    {
        _rb.AddForce(dir * force);
        Invoke("StartDeathTimer", lifeTime);
    }

    void StartDeathTimer()
    {
        Destroy(gameObject);
    }

    public void ApplyAreaOfEffect()
    {
        var obj = GameObject.Instantiate(Resources.Load("Spells/Dummies/ArcaneRepulsionDummy")) as GameObject; 
        var dmm = obj.GetComponent<DMM_ArcaneRepulsion>();
        dmm.Execute(transform, 0.01f, 30, 100, 300, _layMask, _owner);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == Utilities.IntLayers.PLAYER && col.gameObject.name != _owner)
        {
            col.GetComponent<PlayerStats>().TakeDamage(damage, "Spell", _owner);
            ApplyAreaOfEffect();
            CancelInvoke();
            Destroy(gameObject);
        }
    }

}
