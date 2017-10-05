using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePopupGraphic : MonoBehaviour
{
    Collider _col;
    CamRotationController _cam;
    float originalPosY;
    bool isShown;

    void Start()
    {
        originalPosY = transform.position.y;
    }

    public void Activate(CamRotationController cam, Collider col, bool activate)
    {
        _cam = cam;
        _col = col;
        isShown = activate;
        GetComponent<Renderer>().enabled = activate;
    }

    public void Activate(bool activate = false)
    {
        isShown = activate;
        GetComponent<Renderer>().enabled = activate;
    }

    void LateUpdate()
    {
        if(_cam != null && _col != null && isShown)
        {
            transform.LookAt(_cam.transform.position);
            transform.position = _col.ClosestPointOnBounds(_cam.transform.position);
            transform.position = new Vector3(transform.position.x, originalPosY, transform.position.z);
        }
    }
}
