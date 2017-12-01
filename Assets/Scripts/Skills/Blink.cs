using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : ISpell
{
    PlayerMovement _me;

    CastType _castType;

    Transform _enemy;

    float _castTime;
    float _cooldown;

    public bool inSpellCooldown;
    int _manaCost;
    float _minDistance;

    public bool CanBeUsed(float mana)
    {
        return _manaCost < mana && !inSpellCooldown;
    }

    public bool CanBeUsed(float mana, float distance)
    {
        return _manaCost < mana && !inSpellCooldown && distance > _minDistance;
    }

    public void Init()
    {
        _castType = CastType.INSTANT;

        _castTime = 0f;
        _cooldown = 1f;

        _manaCost = 10;
        _minDistance = 10;
    }

    public void Init(PlayerMovement character)
    {
        Init();

        _me = character;
        _enemy = _me.EnemyTransform;
    }

    public void UseSpell()
    {
        if (_me == null) throw new System.Exception("LA CONCHA DE TU MADRE _ME == NULL EXCEPTION");
        if (_enemy == null) _enemy = _me.EnemyTransform;

        var to = GetNearestSpot();

        if (to != default(Transform))
        {
            EventManager.DispatchEvent("ActivateBlinkTrail", new object[] { _me.gameObject.name });
            EventManager.DispatchEvent("StartBlinkFeedback", new object[] { _me.GetComponent<PlayerInput>().GetCamera });

            _me.transform.position = to.position;
            var fTemp = _enemy.position - _me.transform.position;
            _me.transform.forward = new Vector3(fTemp.x, _me.transform.forward.y, _me.transform.forward.z);

            EventManager.DispatchEvent("SpellCasted", new object[] { _manaCost, _me.gameObject.name });
        }
        else
        {
            EventManager.DispatchEvent("SpellCasted", new object[] { 0, _me.gameObject.name });
        }
       
    }

    public void UseSpell(Transform skillPos)
    {
        UseSpell();
    }

    Transform GetNearestSpot()
    {
        var maxDistance = float.MaxValue;
        Transform to = default(Transform);

        foreach (var spot in _enemy.GetComponentInChildren<PlayerBlinkSpots>().blinkSpots)
        {
            var actualDistance = Vector3.Distance(_me.transform.position, spot.transform.position);

            if (actualDistance < maxDistance)
            {
                maxDistance = actualDistance;
                to = spot.transform;
            }
        }

        return to;
    }

    #region Getters
    public float CastTime()
    {
        return _castTime;
    }

    public int GetManaCost()
    {
        return _manaCost;
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
