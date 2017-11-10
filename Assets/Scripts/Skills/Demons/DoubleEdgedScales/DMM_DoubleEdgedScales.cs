using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DMM_DoubleEdgedScales : MonoBehaviour
{
    List<DoubleEdgedScale> _projectiles;
    string _owner;

    public void Init(Transform parent, string owner)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.forward = parent.forward;
        _owner = owner;
    }

    public void Execute()
    {
        LaunchProjectiles();
    }

    public void Execute(Vector3 dir)
    {
        LaunchProjectiles(dir);
    }

    void Update()
    {
        if (transform.parent != null) transform.position = transform.parent.position;
    }

    void CreateProjectiles()
    {
        _projectiles = new List<DoubleEdgedScale>();
        var getDummies = transform.GetComponentsInChildren<Transform>().Where(x => x.name != transform.name);
        foreach (var dmm in getDummies)
        {
            var obj = GameObject.Instantiate(Resources.Load("Spells/Projectiles/DoubleEdgedScale")) as GameObject;
            var scale = obj.GetComponent<DoubleEdgedScale>();
            _projectiles.Add(scale);
            scale.Init(dmm.transform, _owner);
        }
    }

    void LaunchProjectiles()
    {
        CreateProjectiles();
        if (_projectiles == null) return;
        foreach (var dmm in _projectiles)
        {
            dmm.Launch();
        }
        _projectiles = null;
    }

    void LaunchProjectiles(Vector3 dir)
    {
        CreateProjectiles();
        if (_projectiles == null) return;
        foreach (var dmm in _projectiles)
        {
            dmm.Launch(dir);
        }
        _projectiles = null;
    }
}
