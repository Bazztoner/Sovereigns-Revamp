using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GravitationalTelekinesis : ISpell
{
    TelekineticObject _target;

    CastType _castType;

    LayerMask _layerMask;
    GravitationalDummy _dummy;
    RaycastHit rch;

    float _castTime;
    float _cooldown;
    public bool _inSpellCooldown;
    public int manaCost;

    int _getObjectManaCost;
    int _launchObjectManaCost;

    float _radialRange;
    float _pullSpeed;

    bool _hasObject;
    bool _pulled;
    bool inVisionRange;

    float _pullCooldown = 1.2f;
    float _launchCooldown = 2f;

    public void Init()
    {
        _castType = CastType.INSTANT;

        _layerMask = ~(1 << Utilities.IntLayers.TELEKINESISOBJECT
                     | 1 << Utilities.IntLayers.PLAYER
                     | 1 << Utilities.IntLayers.PLAYERCOLLIDER);

        _castTime = 0f;
        _cooldown = 1f;

        _radialRange = 12;
        _pullSpeed = 25;

        _hasObject = false;
        _pulled = false;

        _getObjectManaCost = 20;
        _launchObjectManaCost = 10;

        manaCost = _getObjectManaCost;

        //SpawnDummy();
    }

    [System.Obsolete]
    public void Init(PlayerMovement character)
    {
        throw new System.Exception("Not used");
    }

    void SpawnDummy()
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/GravitationalDummy")) as GameObject;
        _dummy = go.GetComponent<GravitationalDummy>();
    }

    void SpawnDummy(Transform parent)
    {
        var go = GameObject.Instantiate(Resources.Load("Spells/Dummies/GravitationalDummy")) as GameObject;
        _dummy = go.GetComponent<GravitationalDummy>();
        _dummy.Init(parent);
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

        if (_hasObject) LaunchObject(skillPos);
        else GetObject(skillPos);
    }

    void GetObject(Transform skillPos)
    {
        var telekObjs = TelekineticObject.allObjs;
        var objs = new List<TelekineticObject>();
        var charac = skillPos.GetComponentInParent<Player1Input>().transform.position + new Vector3(0f, 1f, 0f);

        foreach (var item in telekObjs)
        {
            var inDistance = Vector3.Distance(charac, item.transform.position) < _radialRange;

            if (item.enabled && inDistance && !item.IsGrabbed && !item.IsReadyToDestroy) objs.Add(item);
        }

        if (!objs.Any()) { return; }

        float minDistance = _radialRange;

        foreach (var o in objs)
        {
            var dst = Vector3.Distance(charac, o.transform.position);

            inVisionRange = Physics.Raycast(charac, (o.transform.position - charac).normalized, out rch, dst, _layerMask);

            Debug.DrawRay(charac, (o.transform.position - charac).normalized, Color.red, 2);
            
            if (dst < minDistance && !inVisionRange)
            {
                minDistance = dst;
                _target = o;
            }
        }

        if (_target == null) return;

        _hasObject = true;
        skillPos.GetComponentInParent<PlayerSkills>().gtHasObject = _hasObject;
        _target.SetGrabbed(_hasObject);

        PullObject(skillPos);
        EventManager.DispatchEvent("ObjectPulling");
       
    }

    void PullObject(Transform skillPos)
    {
        EventManager.DispatchEvent("TelekinesisObjectPulled", new object[]
                                                              {
                                                                   skillPos,
                                                                   skillPos.GetComponentInParent<PlayerParticles>(),
                                                                   _target
                                                              }
                                  );

        _cooldown = _pullCooldown;
        manaCost = _getObjectManaCost;
        _dummy.Execute(_target, _hasObject);
        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, skillPos.GetComponentInParent<Player1Input>().gameObject.name });
        
    }

    void LaunchObject(Transform skillPos)
    {
        EventManager.DispatchEvent("TelekinesisObjectLaunched", new object[]
                                                              {
                                                                    skillPos,
                                                                    skillPos.GetComponentInParent<PlayerParticles>(),
                                                                    _target
                                                              }
                                  );

        _cooldown = _launchCooldown;
        manaCost = _launchObjectManaCost;
        _hasObject = false;
        _dummy.Execute(_target, _hasObject);
        skillPos.GetComponentInParent<PlayerSkills>().gtHasObject = _hasObject;
        _target = null;
        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, skillPos.GetComponentInParent<Player1Input>().gameObject.name });
    }
    
    #region Getters
    public CastType GetCastType()
    {
        return _castType;
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
