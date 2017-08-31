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
public class DestructibleObject : Photon.MonoBehaviour
{
    public int life;
    public float damage;

    [HideInInspector]
    public string nickName;

    public Vector3[] rotAngles;

    public GameObject defaultObject;
    public GameObject destroyedObject;

    public float[] destructionAngles;

    public bool isAlive;

    public static List<DestructibleObject> allObjs;

    public DestructibleType destructibleType;

    private void Start()
    {
        if (allObjs == null) allObjs = new List<DestructibleObject>();
        allObjs.Add(this);
        isAlive = true;

        //EventManager.AddEventListener("DividedScreen", OnDividedScreen);
    }

    void OnDividedScreen(object[] paramsContainer)
    {
        var changeList = allObjs.Select(x => x.GetComponentInChildren<DestructibleImpactArea>());

        var visited = new HashSet<GameObject>();

        foreach (var d in changeList)
        {
            if (!visited.Contains(d.gameObject))
            {
                d.gameObject.layer = Utilities.IntLayers.VISIBLETOP1;
                var newDestImpactArea = GameObject.Instantiate(d.gameObject, d.transform.parent);
                newDestImpactArea.gameObject.layer = Utilities.IntLayers.VISIBLETOP2;
                visited.Add(d.gameObject);
            }
        }

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

        //MARTINNOTEMEENOJES();

        if (destructibleType == DestructibleType.DESTRUCTIBLE) GetComponentInChildren<Collider>().isTrigger = true;

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

        transform.forward = vectorRot;
        //transform.localEulerAngles = vectorRot;
    }

    public void DestroyObject(Vector3 rot, float angle)
    {
        defaultObject.SetActive(false);
        if(rotAngles.Length != 0) CheckRotation(rot, angle);
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
        if (rotAngles.Length != 0) CheckRotation(rot, angle);
        destroyedObject.SetActive(true);

        if (destructibleType == DestructibleType.DESTRUCTIBLE) GetComponentInChildren<Collider>().isTrigger = true;

        isAlive = false;
        var wf = GetComponentInChildren<DestructibleImpactArea>();
        if (wf != null) wf.SetVisible(false);
    }

    [PunRPC]
    public void RpcDestroy(Vector3 rot, float angle, string caster)
    {
        DestroyObject(rot, angle, caster);
    }

    void MARTINNOTEMEENOJES()
    {
        var rnd = UnityEngine.Random.Range(0, destructionAngles.Length);
        transform.Rotate(new Vector3(0, destructionAngles[rnd]));
    }
}

public enum DestructibleType
{
    OBSTRUCTIBLE,
    DESTRUCTIBLE,
    TRANSITION,
    Count
}