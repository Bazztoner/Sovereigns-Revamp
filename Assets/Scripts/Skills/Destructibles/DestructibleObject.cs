using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Esta clase es para cada objeto grande de carácter destructible (Véase Columnas, Puertas, Candelabros)
/// Solo puede ser interactuado por AtractiveTelekinesis
/// "Publicar un internet - Fabián Valladares"
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    public int life;
    public float damage;

    [HideInInspector]
    public string nickName;

    public Vector3[] rotAngles;

    public GameObject defaultObject;
    public GameObject destroyedObject;

    public bool isAlive;

    public static List<DestructibleObject> allObjs;

    public DestructibleType destructibleType;

    private void Start()
    {
        if (allObjs == null) allObjs = new List<DestructibleObject>();
        allObjs.Add(this);
        isAlive = true;

        EventManager.AddEventListener(GameEvents.DividedScreen, OnDividedScreen);
        EventManager.AddEventListener(GameEvents.EndMatch, OnEndMatch);

    }

    void OnEndMatch(object[] paramsContainer)
    {
        EventManager.RemoveEventListener(GameEvents.DividedScreen, OnDividedScreen);
        EventManager.RemoveEventListener(GameEvents.EndMatch, OnEndMatch);
    }

    void OnDividedScreen(object[] paramsContainer)
    {
        var changeList = allObjs.Where(x => x.destructibleType != DestructibleType.TRANSITION)
                                .Select(x => x.GetComponentInChildren<DestructibleImpactArea>());

        foreach (var d in changeList)
        {
            if (d != null)
            {
                d.gameObject.layer = Utilities.IntLayers.VISIBLETOP1;
                foreach (Transform c in d.transform)
                {
                    c.gameObject.layer = Utilities.IntLayers.VISIBLETOP1;
                }
            }
        }
    }

    public static void DeleteAllObjs()
    {
        allObjs = null;
    }

    public void TakeDamage(int damage)
    {
        life -= damage;

        if (life <= 0) DestroyObject();
    }

    public void DestroyObject()
    {
        defaultObject.SetActive(false);
        destroyedObject.SetActive(true);

        if (destructibleType == DestructibleType.DESTRUCTIBLE) GetComponentInChildren<Collider>().isTrigger = true;
        else if (destructibleType == DestructibleType.TRANSITION)
        {
            var boundingBox = GetComponentInChildren<DestructibleBoundingBox>();
            if (boundingBox != null)
            {
                boundingBox.enabled = false;
            }
        }

        isAlive = false;
        var wf = GetComponentsInChildren<DestructibleImpactArea>();
        foreach (var item in wf)
        {
            if (item != null) item.SetVisible(false);
        }
    }

    void CheckRotation(Vector3 rot, float angle)
    {
        var minAngle = angle; 
        Vector3 vectorRot = transform.forward;

        foreach (var a in rotAngles)
        {
            var tempAngle = Vector3.Angle(rot, a);
            if (tempAngle < minAngle)
            {
                minAngle = tempAngle;
                vectorRot = a;
            }
        }

        ///Para mi estimadísimo señor Valladares: Aprecio infinitamente que haya aceptado mi sincera idea de que
        ///hemos de hardcodear cuando la vida nos tira al suelo. Solo nosotros decidimos si nos levantamos ante ella.
        if((this.gameObject.name == "SingleDoor" || this.gameObject.name == "SingleDoor (1)") && Vector3.Angle(this.transform.forward, rot) >= 90) transform.forward *= -1;
        else transform.rotation = Quaternion.Euler(vectorRot);
    }

    public void DestroyObject(Vector3 rot, float angle)
    {
        defaultObject.SetActive(false);
        if(rotAngles.Length > 0) CheckRotation(rot, angle);
        destroyedObject.SetActive(true);

        if (destructibleType == DestructibleType.DESTRUCTIBLE) GetComponentInChildren<Collider>().isTrigger = true;

        isAlive = false;
        var wf = GetComponentInChildren<DestructibleImpactArea>();
        if (wf != null) wf.SetVisible(false);
    }

    //Hice una sobrecarga para probar algo
    //Esto lo hiciste vos Fabi? No me acuerdo si lo hice yo xD
    //Hola Pablo (?
    public void DestroyObject(Vector3 rot, float angle, string caster)
    {
        nickName = caster;
        defaultObject.SetActive(false);
        if (rotAngles.Length > 0) CheckRotation(rot, angle);
        destroyedObject.SetActive(true);

        if (destructibleType == DestructibleType.DESTRUCTIBLE) GetComponentInChildren<Collider>().isTrigger = true;

        isAlive = false;
        var wf = GetComponentInChildren<DestructibleImpactArea>();
        if (wf != null) wf.SetVisible(false);
    }

    public void RpcDestroy(Vector3 rot, float angle, string caster)
    {
        DestroyObject(rot, angle, caster);
    }
}

public enum DestructibleType
{
    OBSTRUCTIBLE,
    DESTRUCTIBLE,
    TRANSITION,
    Count
}