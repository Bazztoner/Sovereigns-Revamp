using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMM_ArcaneOrb : MonoBehaviour
{
    bool _aiming;
    ArcaneOrb _orb;
    string _owner;

    public void Init(Transform parent, string owner)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.forward = parent.forward;
        _owner = owner;

        EventManager.AddEventListener(SkillEvents.ArcaneOrbDestroyedByLifeTime, OnOrbDestruction);
    }

    public void Execute()
    {
        if (!_aiming) CreateOrb();
        else LaunchOrb();
    }

    public void Execute(Vector3 dir)
    {
        if (!_aiming) CreateOrb();
        else LaunchOrb(dir);
    }

    void Update()
    {
        if (transform.parent != null && _aiming) transform.position = transform.parent.position;
    }

    void CreateOrb()
    {
        _aiming = true;
        var obj = GameObject.Instantiate(Resources.Load("Spells/Projectiles/ArcaneOrb")) as GameObject;
        _orb = obj.GetComponent<ArcaneOrb>();
        _orb.Init(transform, _owner);
    }

    void LaunchOrb()
    {
        if (_orb == null) return;
        _aiming = false;
        _orb.Launch(transform.forward);
        _orb.transform.SetParent(null);
        _orb = null;
    }

    void LaunchOrb(Vector3 dir)
    {
        if (_orb == null) return;
        _aiming = false;
        _orb.Launch(dir);
        _orb.transform.SetParent(null);
        _orb = null;
    }

    void OnOrbDestruction(object[] paramsContainer)
    {
        if (_orb == (ArcaneOrb)paramsContainer[0])
        {
            _aiming = false;
            _orb = null;
            EventManager.DispatchEvent(SkillEvents.ArcaneDummyDestroyedByLifeTime, new object[] { this });
            Destroy(gameObject);
        }
    }
}
