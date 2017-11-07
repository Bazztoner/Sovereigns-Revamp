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
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        _owner = owner;
    }

    public void Execute()
    {
        if (!_aiming) CreateOrb();
        else LaunchOrb();
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
        _orb = null;
    }
}
