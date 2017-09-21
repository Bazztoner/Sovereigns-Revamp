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
        EventManager.AddEventListener("GameFinished", OnGameFinished);
    }

    /// <summary>Running animations</summary>
    private void OnRunningAnimations(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            _anim.SetFloat("xMovement", (float)paramsContainer[1]);
            _anim.SetFloat("yMovement", (float)paramsContainer[2]);
        }
    }

    /// <summary>Rolling Animation</summary>
    private void OnRollingAnimation(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("isRolling", (bool)paramsContainer[1]);
    }

    #region Cambios Iván 21/9
    //Agregada la línea de blockUp
    /// <summary>Blocking Animation</summary>
    private void OnBlocking(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            _anim.SetBool("isBlocking", (bool)paramsContainer[1]);
            _anim.SetBool("blockUp", (bool)paramsContainer[2]);
        }
    }
    #endregion

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
        
        if ((bool)paramsContainer[1])
        {
            _anim.SetBool("isRolling", false);
            _anim.SetBool("isBlocking", false);
        }
    }

    /// <summary>Makes the character stop running after the match finished</summary>
    private void OnGameFinished(params object[] paramsContainer)
    {
        _anim.SetBool("runForward", false);
        _anim.SetBool("runRight", false);
        _anim.SetBool("runLeft", false);
    }
}
