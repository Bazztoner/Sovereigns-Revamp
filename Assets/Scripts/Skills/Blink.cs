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

    public bool _inSpellCooldown;
    int manaCost;

    public void Init()
    {
        throw new System.Exception("Not used");
    }

    public void Init(PlayerMovement character)
    {
        _castType = CastType.INSTANT;

        _me = character;
        _enemy = _me.EnemyTransform;

        _castTime = 0f;
        _cooldown = 1f;

        manaCost = 10;
    }

    public void UseSpell()
    {
        if (_me == null) throw new System.Exception("LA CONCHA DE TU MADRE _ME == NULL EXCEPTION");
        if (_enemy == null) _enemy = _me.EnemyTransform;

        var to = GetNearestSpot();

        if (to != default(Transform))
        {
            _me.transform.position = to.position;
            var fTemp = _enemy.position - _me.transform.position;
            _me.transform.forward = new Vector3(fTemp.x, _me.transform.forward.y, _me.transform.forward.z);
            //_me.transform.forward = new Vector3(_me.transform.forward.x, fTemp.y, _me.transform.forward.z);
            //_me.transform.forward = new Vector3(_me.transform.forward.x, _me.transform.forward.y, fTemp.z);
            EventManager.DispatchEvent("SpellCasted", new object[] { manaCost });
        }
        else
        {
            EventManager.DispatchEvent("SpellCasted", new object[] { 0 });
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

        foreach (var spot in _enemy.GetComponent<PlayerBlinkSpots>().blinkSpots)
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
        return manaCost;
    }

    public void EnterInCooldown()
    {
        _inSpellCooldown = true;
    }
    public void ExitFromCooldown()
    {
        _inSpellCooldown = false;
    }
    public bool IsInCooldown()
    {
        return _inSpellCooldown;
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
