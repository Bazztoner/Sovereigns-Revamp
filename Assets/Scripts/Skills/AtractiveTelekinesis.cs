using System;
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

    public bool _inSpellCooldown;
    int manaCost;

    float _radialRange;
    float _damage;

    bool _hasObject;
    bool _pulled;

    float _pullCooldown = .3f;
    float _launchCooldown = 2f;

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

        manaCost = 150;
    }

    public void Init(PlayerMovement character)
    {
        var camContainter = character.GetComponent<Player1Input>().GetCamera;
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
        var camContainter = me.GetComponentInParent<Player1Input>().GetCamera;
        _target = camContainter.CurrentTarget;

        if (_target == null) return;

        PullObject(me);
       
    }

    void PullObject(CamRotationController cam)
    {
        if (PhotonNetwork.offlineMode) _target.DestroyObject(cam.transform.forward, cam.AngleVision);
        else _target.photonView.RPC("RpcDestroy", PhotonTargets.All, cam.transform.forward, cam.AngleVision, PhotonNetwork.player.NickName);

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost });
    }

    void PullObject(Transform me)
    {
        var camContainter = me.GetComponentInParent<Player1Input>().GetCamera;
        _target = camContainter.CurrentTarget;
        if (PhotonNetwork.offlineMode) _target.DestroyObject(camContainter.transform.forward, camContainter.AngleVision);
        else _target.photonView.RPC("RpcDestroy", PhotonTargets.All, camContainter.transform.forward, camContainter.AngleVision, PhotonNetwork.player.NickName);

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost, me.GetComponentInParent<Player1Input>().gameObject.name });
    }

    void PullObject()
    {
        var cam = GameObject.FindObjectOfType<CamRotationController>();
        _target = cam.CurrentTarget;

        if (PhotonNetwork.offlineMode) _target.DestroyObject(cam.transform.forward, cam.AngleVision);
        else _target.photonView.RPC("RpcDestroy", PhotonTargets.All, cam.transform.forward, cam.AngleVision, PhotonNetwork.player.NickName);

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost });
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
