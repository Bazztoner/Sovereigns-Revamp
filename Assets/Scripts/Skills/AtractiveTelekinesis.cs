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

        manaCost = 32;
    }

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

        if (_target.isAlive) PullObject(camContainter);
       
    }

    void PullObject(CamRotationController cam)
    {
        if (PhotonNetwork.offlineMode) _target.DestroyObject(cam.transform.forward, cam.AngleVision);
        else _target.photonView.RPC("RpcDestroy", PhotonTargets.All, cam.transform.forward, cam.AngleVision, PhotonNetwork.player.NickName);

        _target = null;
        _hasObject = false;

        EventManager.DispatchEvent("SpellCasted", new object[] { manaCost });
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

    #region Deprecated Methods
    [System.Obsolete]
    public void Init(PlayerMovement character)
    {
        throw new System.Exception("Not used");
    }

    /*void GetObject(Transform me)
    {
        var objs = new List<DestructibleObject>();

        foreach (var item in _destructObjs)
        {
            if (Vector3.Distance(me.position, item.transform.position) < _radialRange && item.isAlive) objs.Add(item);
        }

        if (!objs.Any()) { return; }

        float minDistance = _radialRange;

        float nearDistance = _radialRange;

        foreach (var o in objs)
        {
            //Filtrar por distancia
            //Filtrar por Angulo
            //Filtrar por colision o falta de
            var tempDistance = Vector3.Distance(o.transform.position, me.position);

            var tempAngle = Vector3.Angle(me.forward, (o.transform.position - me.position).normalized);

            RaycastHit rch;
            var objectsBetween = !Physics.Raycast(me.position, me.position - o.transform.position, out rch, 100, _layerMask);

            Debug.DrawLine(me.position, o.transform.position, Color.blue, 1);
            // Debug.Log("Item: " + o.name + " | World Pos: " + o.transform.position + " | Dist: " + tempDistance + " | objects between? " + objectsBetween + " | Angle: " + tempAngle);
            
            if (tempDistance < nearDistance 
                && o.GetComponent<DestructibleObject>().isAlive 
                && objectsBetween && tempAngle < 75)
            {
                nearDistance = tempDistance;
                _target = o.GetComponent<DestructibleObject>();
                //Debug.Log("Angle!: " + tempAngle); 
            }
        }

        if (_target == null) return;

        _hasObject = true;


        var camContainter = me.GetComponentInParent<Player1Input>().GetCamera;

        PullObject(camContainter);
    }*/
    #endregion
}
