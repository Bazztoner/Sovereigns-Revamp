using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SK_ArcaneOrb : ISpell
{
    CastType _castType;

    LayerMask _layerMask;
    DMM_ArcaneOrb _dummy;
    RaycastHit _rch;

    string _owner;
    float _castTime;
    float _cooldown;
    public bool inSpellCooldown;
    public int manaCost;

    bool _hasOrb;

    int _orbCreationManaCost = 50;
    int _launchManaCost = 0;

    float _orbCreationCooldown = .1f;
    float _launchCooldown = 2f;

    public void Init()
    {
        _castType = CastType.INSTANT;

        _layerMask = ~(1 << Utilities.IntLayers.TELEKINESISOBJECT
                     | 1 << Utilities.IntLayers.PLAYER
                     | 1 << Utilities.IntLayers.PLAYERCOLLIDER);

        _castTime = 0f;
        _cooldown = _orbCreationCooldown;

        manaCost = _orbCreationManaCost;
    }

    public void Init(PlayerMovement character)
    {
        _owner = character.gameObject.name;
        Init();
    }

    void SpawnDummy()
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/ArcaneOrbDummy")) as GameObject;
        _dummy = go.GetComponent<DMM_ArcaneOrb>();
    }

    void SpawnDummy(Transform parent)
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/ArcaneOrbDummy")) as GameObject;
        _dummy = go.GetComponent<DMM_ArcaneOrb>();
        _dummy.Init(parent, _owner);
    }

    void RelocateDummy(Transform skillPos)
    {
        _dummy.transform.position = skillPos.position;
    }

    public void UseSpell() { }

    public void UseSpell(Transform skillPos)
    {
        if (skillPos == null) throw new System.Exception("LA CONCHA DE TU MADRE _ME == NULL EXCEPTION");

        if (_dummy == null)
        {
            SpawnDummy(skillPos);
        }
        RelocateDummy(skillPos);
        _dummy.Execute();

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, _owner });

        if (!_hasOrb)
        {
            manaCost = _launchManaCost;
            _cooldown = _launchCooldown;
        }
        else
        {
            manaCost = _orbCreationManaCost;
            _cooldown = _orbCreationCooldown;
        }
        _hasOrb = !_hasOrb;
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
        return _cooldown;
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

//Pablo no visita estos parajes.
//Aguante Malón la concha de tu madre
//
//Todos los TelekObj tienen que tener el pivot en el medio por el RayCast
//
//Ja no hay mas telekobjs xd xd xd xd xd
// :((((((((((((((
