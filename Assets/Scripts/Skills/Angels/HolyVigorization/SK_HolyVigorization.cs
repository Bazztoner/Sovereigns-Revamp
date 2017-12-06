using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_HolyVigorization : ISpell
{
    CastType _castType;
    CamRotationController _cam;
    PlayerMovement _char;

    string _owner;
    float _castTime = 1;
    float _cooldown = 13;
    public bool inSpellCooldown;
    public int manaCost = 95;
    float _duration = 3f;
    float _selfDamageIncrease = 1.85f;
    float _attackSpeedIncrease = 1.3f;
    bool _isActive;

    public bool CanBeUsed(float mana)
    {
        return manaCost < mana && !inSpellCooldown && !_isActive;
    }

    public bool CanBeUsed(float mana, float distance)
    {
        return manaCost < mana && !inSpellCooldown && !_isActive;
    }

    public void Init()
    {
        _castType = CastType.DELAYED;
        EventManager.AddEventListener(SkillEvents.HolyVigorizationEnded, OnEndSpell);
    }


    void OnEndSpell(object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == _owner)
        {
            _isActive = false;
        }
    }

    public void Init(PlayerMovement character)
    {
        _owner = character.gameObject.name;
        _cam = character.GetComponent<PlayerInput>().GetCamera;
        _char = character;
        Init();
    }

    public void UseSpell()
    {
        Execute();

        EventManager.DispatchEvent(SkillEvents.SpellCasted, new object[] { manaCost, _owner });
        EventManager.DispatchEvent(SkillEvents.HolyVigorizationCasted, new object[] { _owner, _duration, _selfDamageIncrease, _attackSpeedIncrease });
    }

    public void UseSpell(Transform skillPos)
    {
        UseSpell();
    }

    void Execute()
    {
        _isActive = true;
    }

    #region Getters
    public CastType GetCastType()
    {
        return _castType;
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
        return _cooldown + _duration;
    }

    public int GetManaCost()
    {
        return manaCost;
    }

    public float CastTime()
    {
        return _castTime;
    }
    #endregion
}
