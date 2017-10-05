using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDummy : MonoBehaviour
{
    Animator _an;
    Rigidbody _rb;
    public bool isLaunched;

    public Animator GetAnimator { get { return _an; } }

    public Rigidbody GetRigidbody { get { return _rb; } }

    void Start ()
    {
        _an = GetComponent<Animator>();
	}
	
	void Update ()
    {
		
	}

    public void Animate(string animation)
    {
        if (_an == null) _an = GetComponent<Animator>();
        _an.SetBool(animation, true);
    }

    void OnTriggerEnter(Collider c)
    {
        if (isLaunched && c.gameObject.layer == Utilities.IntLayers.DESTRUCTIBLEOBJECT)
        {
            EventManager.DispatchEvent("DummyCollidedWithDestructible", new object[] { c.gameObject.GetComponent<DestructibleObject>() });
            GetComponent<Collider>().isTrigger = false;
        }
    }

    void OnStartLerp()
    {

    }

    void OnAttackExit()
    {
        _an.SetBool("Attack", false);
    }

    void OnDamageExit()
    {
        _an.SetBool("Damaged", false);
    }
}
