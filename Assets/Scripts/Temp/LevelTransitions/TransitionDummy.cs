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
        _an = GetComponentInChildren<Animator>();
	}
	
	void Update ()
    {
		
	}

    public void MakeAttack()
    {
        var anim = Animator.StringToHash("YY");
        _an.Play(anim);
    }

    void OnTriggerEnter(Collider c)
    {
        if (isLaunched && c.gameObject.layer == Utilities.IntLayers.DESTRUCTIBLEOBJECT)
        {
            EventManager.DispatchEvent("DummyCollidedWithDestructible", new object[] { c.gameObject.GetComponent<DestructibleObject>() });
            GetComponent<Collider>().isTrigger = false;
        }
    }

}
