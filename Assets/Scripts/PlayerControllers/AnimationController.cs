using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
    
    private Animator _anim;

    void Start()
    {
        _anim = this.GetComponent<Animator>();

        AddEvents();
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("RunningAnimations", OnRunningAnimations);
        EventManager.AddEventListener("RollingAnimation", OnRollingAnimation);
        EventManager.AddEventListener("Blocking", OnBlocking);
        EventManager.AddEventListener("X", OnX);
        EventManager.AddEventListener("Y", OnY);
        EventManager.AddEventListener("IsDead", OnIsDead);
        EventManager.AddEventListener("IsDamaged", OnIsDamaged);
    }

    /// <summary>Running animations</summary>
    private void OnRunningAnimations(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            _anim.SetBool("runForward", (bool)paramsContainer[1]);
            _anim.SetBool("runRight", (bool)paramsContainer[2]);
            _anim.SetBool("runLeft", (bool)paramsContainer[3]);
            _anim.SetFloat("speedMult", (float)paramsContainer[4]);
        }
    }

    /// <summary>Rolling Animation</summary>
    private void OnRollingAnimation(params object[] paramsContainer)
    {
        if((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("isRolling", (bool)paramsContainer[1]);
    }

    /// <summary>Blocking Animation</summary>
    private void OnBlocking(params object[] paramsContainer)
    {
        if((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("isBlocking", (bool)paramsContainer[1]);
    }

    /// <summary>Light Attack Animation</summary>
    private void OnX(params object[] paramsContainer)
    {
        if((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("X", (bool)paramsContainer[1]);
    }

    /// <summary>Heavy Attack Animation</summary>
    private void OnY(params object[] paramsContainer)
    {
        if((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("Y", (bool)paramsContainer[1]);
    }

    /// <summary>Death Animation</summary>
    private void OnIsDead(params object[] paramsContainer)
    {
        if(this.gameObject.name == (string)paramsContainer[0])
            _anim.SetBool("isDead", (bool)paramsContainer[1]);
    }

    /// <summary>Damage Animation</summary>
    private void OnIsDamaged(params object[] paramsContainer)
    {
        if(this.gameObject.name == (string)paramsContainer[0])
            _anim.SetBool("isDamaged", (bool)paramsContainer[1]);
    }
}
