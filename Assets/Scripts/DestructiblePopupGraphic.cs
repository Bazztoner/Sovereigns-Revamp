using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePopupGraphic : MonoBehaviour
{
    Collider _col;
    CamRotationController _cam;
    float _originalPosY;
    bool _isShown;
    public bool isBugged;

    void Start()
    {
        _originalPosY = transform.position.y;
    }

    public void Activate(CamRotationController cam, Collider col, bool activate)
    {
        _cam = cam;
        _col = col;
        _isShown = activate;
        GetComponent<Renderer>().enabled = activate;
    }

    public void Activate(bool activate = false)
    {
        _isShown = activate;
        GetComponent<Renderer>().enabled = activate;
    }

    void LateUpdate()
    {
        if (_cam != null && _col != null && _isShown)
        {
            if (!isBugged)
            {
                LookAtTarget();
                var ray = new Ray(_cam.transform.position, _col.transform.position - _cam.transform.position);
                RaycastHit rch;
                var raycast = _col.Raycast(ray, out rch, Vector3.Distance(_cam.transform.position, _col.transform.position));
                var closestPoint = rch.point;
                transform.position = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
            }
            else
            {
                var camFwd = _cam.transform.TransformDirection(_cam.transform.forward);
                var myFwd = transform.TransformDirection(transform.forward);
                var ngl = Vector3.Angle(myFwd, camFwd);

                print("EL ÁNGULO ES " + ngl.ToString());

                if (ngl >= 10)
                {
                    transform.Rotate(0, ngl, 0);
                }
            }
        }
    }

    void LookAtTarget()
    {
        var direction = (_cam.transform.position - transform.position).normalized;
        transform.forward = new Vector3(direction.x, 0f, direction.z);
    }
}
