using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Obsolete("SCRAPPED", true)]
public class RepulsiveTelekinesis : ISpell
{
    CastType _castType;

    LayerMask _layerMask;

    float _castTime;

    float _cooldown;
    public bool _inSpellCooldown;
    public int manaCost;

    float _radialRange;
    float _verticalForce;
    float _radialForce;

    public void Init()
    {
        _castType = CastType.DELAYED;

        _layerMask = 1 << LayerMask.NameToLayer("TelekinesisObject");

        _castTime = 1f;

        _cooldown = 3f;

        _radialRange = 7;
        _verticalForce = 250;
        _radialForce = 1500;

        manaCost = 50;
    }

    public void Init(PlayerMovement character)
    {
        Init();
    }

    public void UseSpell() { }

    public void UseSpell(Transform skillPos)
    {
        if (skillPos == null) throw new System.Exception("LA CONCHA DE TU MADRE _ME == NULL EXCEPTION");

        SpawnDummy(skillPos);
    }

    void SpawnDummy(Transform skillPos)
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/RepulsiveDummy"), skillPos.position, Quaternion.identity) as GameObject;
        var dummy = go.GetComponent<DMM_ArcaneRepulsion>();

        var caster = skillPos.GetComponentInParent<PlayerMovement>();
        var enemy = caster.EnemyTransform.gameObject;

        dummy.Execute(skillPos, _castTime, _radialRange, _verticalForce, _radialForce, _layerMask, "ESTE SKILL NO ANDA XD");

        EventManager.DispatchEvent("RepulsiveTelekinesisCasted", new object[] { skillPos.position, skillPos.GetComponentInParent<PlayerParticles>() });

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, skillPos.GetComponentInParent<PlayerInput>().gameObject.name });
    }


    #region Getters
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
    public int GetManaCost()
    {
        return manaCost;
    }

    public CastType GetCastType()
    {
        return _castType;
    }

    public float CastTime() // TODO: Para que haces estas funciones al pedo cuando podes usar getters????
    {
        return _castTime;
    }
    #endregion

}

