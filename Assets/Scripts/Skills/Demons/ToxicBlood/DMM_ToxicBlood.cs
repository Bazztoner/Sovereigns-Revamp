using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMM_ToxicBlood : MonoBehaviour
{
    ToxicBlood _proyectile;
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
        LaunchProyectile();
    }

    public void Execute(Vector3 dir)
    {
        LaunchProyectile(dir);
    }

    void Update()
    {
        if (transform.parent != null) transform.position = transform.parent.position;
    }

    void CreateProyectile()
    {
        var obj = GameObject.Instantiate(Resources.Load("Spells/Projectiles/ToxicBlood")) as GameObject;
        _proyectile = obj.GetComponent<ToxicBlood>();
        _proyectile.Init(transform, _owner);
    }

    void LaunchProyectile()
    {
        CreateProyectile();
        if (_proyectile == null) return;
        _proyectile.Launch(transform.forward);
        _proyectile = null;
    }

    void LaunchProyectile(Vector3 dir)
    {
        CreateProyectile();
        if (_proyectile == null) return;
        _proyectile.Launch(dir);
        _proyectile = null;
    }
}
