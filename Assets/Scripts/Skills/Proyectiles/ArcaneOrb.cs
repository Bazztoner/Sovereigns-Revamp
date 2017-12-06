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
    bool _aiming;

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
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.forward = transform.parent.forward;
        _owner = owner;
        _aiming = true;
    }

    public void Launch(Vector3 dir)
    {
        _rb.AddForce(dir * throwForce);
        _aiming = false;
        Invoke("DeathTimer", lifeTime);
    }

    public void Launch(Vector3 dir, float force)
    {
        _rb.AddForce(dir * force);
        _aiming = false;
        Invoke("DeathTimer", lifeTime);
    }

    void LateUpdate()
    {
        if (transform.parent != null && _aiming) _rb.MovePosition(transform.parent.position);
    }

    void DeathTimer()
    {
        EventManager.DispatchEvent(SkillEvents.ArcaneOrbDestroyedByLifeTime, new object[] { this });
        ApplyAreaOfEffect();
        CancelInvoke();
        Destroy(gameObject);
    }

    public void ApplyAreaOfEffect()
    {
        var obj = GameObject.Instantiate(Resources.Load("Spells/Dummies/ArcaneRepulsionDummy")) as GameObject;
        var dmm = obj.GetComponent<DMM_ArcaneRepulsion>();
        dmm.Execute(transform, 0.01f, 5, 320, 600, _layMask, _owner);
    }

    void OnTriggerEnter(Collider col)
    {
        if (!_aiming)
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
                    ApplyAreaOfEffect();
                    CancelInvoke();
                    Destroy(gameObject);
                }
            }
        }

    }

}
