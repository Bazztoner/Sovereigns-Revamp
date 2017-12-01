using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SK_ToxicBlood : ISpell
{
    public const string spellName = "ToxicBlood";
    CastType _castType;

    LayerMask _layerMask;
    DMM_ToxicBlood _dummy;
    RaycastHit _rch;
    CamRotationController _cam;
    PlayerMovement _char;

    string _owner;
    float _castTime = 0;
    float _cooldown = 7;
    public bool inSpellCooldown;
    public int manaCost = 75;

    public bool CanBeUsed(float mana)
    {
        return manaCost < mana && !inSpellCooldown;
    }

    public bool CanBeUsed(float mana, float distance)
    {
        return manaCost < mana && !inSpellCooldown;
    }

    public void Init()
    {
        _castType = CastType.INSTANT;

        _layerMask = ~(1 << Utilities.IntLayers.TELEKINESISOBJECT
                     | 1 << Utilities.IntLayers.PLAYER
                     | 1 << Utilities.IntLayers.PLAYERCOLLIDER);
    }

    public void Init(PlayerMovement character)
    {
        _owner = character.gameObject.name;
        _cam = character.GetComponent<PlayerInput>().GetCamera;
        _char = character;
        Init();
    }

    void SpawnDummy()
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/ToxicBloodDummy")) as GameObject;
        _dummy = go.GetComponent<DMM_ToxicBlood>();
    }

    void SpawnDummy(Transform parent)
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/ToxicBloodDummy")) as GameObject;
        _dummy = go.GetComponent<DMM_ToxicBlood>();
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

        _dummy.Execute(GetLaunchDirection());

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, _owner });
    }

    public Vector3 GetLaunchDirection()
    {
        return _char.Enemy.position - _char.transform.position;
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
