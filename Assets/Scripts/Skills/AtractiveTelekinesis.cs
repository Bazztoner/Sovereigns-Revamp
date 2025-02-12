﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AtractiveTelekinesis : ISpell
{
    List<DestructibleObject> _destructObjs;
    DestructibleObject _target;

    CastType _castType;

    LayerMask _layerMask;

    float _castTime;
    float _cooldown;

    public bool inSpellCooldown;
    int manaCost;

    float _radialRange;
    float _damage;

    bool _hasObject;
    bool _pulled;

    float _pullCooldown = .3f;
    float _launchCooldown = 2f;

    public bool projecting;

    public void Init()
    {
        _castType = CastType.INSTANT;

        _layerMask = 0 << LayerMask.NameToLayer("DestructibleObject");

        _castTime = 0f;
        _cooldown = 1f;

        _radialRange = 12;
        _damage = 5;

        _hasObject = false;
        _pulled = false;

        manaCost = 125;
    }

    public bool CanBeUsed(float mana)
    {
        return manaCost < mana && !inSpellCooldown;
    }

    public bool CanBeUsed(float mana, float distance)
    {
        return manaCost < mana && !inSpellCooldown;
    }

    public void Init(PlayerMovement character)
    {
        var camContainter = character.GetComponent<PlayerInput>().GetCamera;
        _target = camContainter.CurrentTarget;
        Init();
    }

    [System.Obsolete("Not used")]
    public void UseSpell() { }

    public void UseSpell(Transform me)
    {
        if (_destructObjs == null) _destructObjs = DestructibleObject.allObjs;
        if (me.gameObject == null) throw new System.Exception("LA CONCHA DE TU MADRE _ME == NULL EXCEPTION");

        GetObject(me);
    }

    void GetObject(Transform me)
    {
        var camContainter = me.GetComponentInParent<PlayerInput>().GetCamera;
        _target = camContainter.CurrentTarget;

        if (_target == null) return;

        PullObject(me);

    }

    void PullObject(CamRotationController cam)
    {
        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent(SkillEvents.SpellCasted, new object[] { manaCost });
    }

    void PullObject(Transform me)
    {
        var camContainter = me.GetComponentInParent<PlayerInput>().GetCamera;
        _target = camContainter.CurrentTarget;

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent(SkillEvents.SpellCasted, new object[] { manaCost, me.GetComponentInParent<PlayerInput>().gameObject.name });
    }

    void PullObject()
    {
        var cam = GameObject.FindObjectOfType<CamRotationController>();
        _target = cam.CurrentTarget;

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent(SkillEvents.SpellCasted, new object[] { manaCost });
    }

    #region "Getters"
    public float CastTime()
    {
        return _castTime;
    }

    public int GetManaCost()
    {
        return manaCost;
    }

    public void EnterInCooldown()
    {
        inSpellCooldown = true;
    }
    public void ExitFromCooldown()
    {
        inSpellCooldown = false;
    }
    public bool IsInCooldown()
    {
        return inSpellCooldown;
    }

    public float CooldownTime()
    {
        return _cooldown;
    }

    public CastType GetCastType()
    {
        return _castType;
    }
    #endregion
}
