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
        print(_an);
	}
	
	void Update ()
    {
		
	}

    public void Animate(string animation)
    {
        //var anim = Animator.StringToHash("YY");
        _an.Play(animation);
    }

    void OnTriggerEnter(Collider c)
    {
        if (isLaunched && c.gameObject.layer == Utilities.IntLayers.DESTRUCTIBLEOBJECT)
        {
            EventManager.DispatchEvent("DummyCollidedWithDestructible", new object[] { c.gameObject.GetComponent<DestructibleObject>() });
            GetComponent<Collider>().isTrigger = false;
        }
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
