using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

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
        EventManager.AddEventListener("GuardBreak", OnGuardBreak);
        EventManager.AddEventListener("StunParticle", OnStun);
        EventManager.AddEventListener("StopStun", OnStopStun);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
    }

    //For the fucking block+Y
    void OnAttackEnter()
    {
        EventManager.DispatchEvent("AttackEnter", new object[] { gameObject.name, GetComponent<PlayerCombat>().heavyAttackDamage });
    }

    void OnStun(object[] paramsContainer)
    {
        if (gameObject.name == (string)paramsContainer[0])
        {
            _anim.SetBool("X", false);
            _anim.SetBool("Y", false);
            _anim.SetBool("isStunned", true);
            _anim.SetFloat("stunSpeed", .6f);
        }
    }

    void OnStopStun(object[] paramsContainer)
    {
        if (gameObject.name == (string)paramsContainer[0])
        {
            _anim.SetBool("isStunned", false);
        }
    }

    void OnGuardBreak(object[] paramsContainer)
    {
        if (gameObject.name == (string)paramsContainer[0]) _anim.SetBool("guardBreak", true);
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

    /// <summary>Blocking Animation</summary>
    private void OnBlocking(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            _anim.SetBool("isBlocking", (bool)paramsContainer[1]);
            _anim.SetBool("blockUp", (bool)paramsContainer[2]);
        }
    }

    /// <summary>Light Attack Animation</summary>
    private void OnX(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("X", (bool)paramsContainer[1]);
    }

    /// <summary>Heavy Attack Animation</summary>
    private void OnY(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
            _anim.SetBool("Y", (bool)paramsContainer[1]);
    }

    /// <summary>Death Animation</summary>
    private void OnIsDead(params object[] paramsContainer)
    {
        _anim.SetFloat("xMovement", 0);
        _anim.SetFloat("yMovement", 0);

        if (this.gameObject.name == (string)paramsContainer[0])
            _anim.SetBool("isDead", (bool)paramsContainer[1]);
    }

    /// <summary>Damage Animation</summary>
    /// [2] Can the entity cancel his actual attack by beign damaged?
    private void OnIsDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
            _anim.SetBool("isDamaged", (bool)paramsContainer[1]);

        if ((bool)paramsContainer[1])
        {
            _anim.SetBool("isRolling", false);
            _anim.SetBool("isBlocking", false);
            _anim.SetBool("blockUp", false);
        }
    }

    /// <summary>Makes the character stop running after the match finished</summary>
    private void OnGameFinished(params object[] paramsContainer)
    {
        _anim.SetFloat("xMovement", 0);
        _anim.SetFloat("yMovement", 0);
        _anim.SetBool("runForward", false);
        _anim.SetBool("runRight", false);
        _anim.SetBool("runLeft", false);
        _anim.SetBool("isRolling", false);
        _anim.SetBool("isBlocking", false);
        _anim.SetBool("blockUp", false);
    }

    /// <summary>Resets the animator to start over the game</summary>
    private void OnRestartRound(params object[] paramsContainer)
    {
        _anim.SetBool("isBlocking", false);
        _anim.SetBool("isRolling", false);
        _anim.SetBool("X", false);
        _anim.SetBool("Y", false);
        _anim.SetBool("isDamaged", false);
        _anim.SetBool("isDead", false);
        _anim.SetBool("transitionDamage", false);
        _anim.SetBool("blockUp", false);
        _anim.SetFloat("xMovement", 0f);
        _anim.SetFloat("yMovement", 0f);

        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener("RunningAnimations", OnRunningAnimations);
            EventManager.RemoveEventListener("RollingAnimation", OnRollingAnimation);
            EventManager.RemoveEventListener("Blocking", OnBlocking);
            EventManager.RemoveEventListener("X", OnX);
            EventManager.RemoveEventListener("Y", OnY);
            EventManager.RemoveEventListener("IsDead", OnIsDead);
            EventManager.RemoveEventListener("IsDamaged", OnIsDamaged);
            EventManager.RemoveEventListener("GuardBreak", OnGuardBreak);
            EventManager.RemoveEventListener("StunParticle", OnStun);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);
        }
    }
}
