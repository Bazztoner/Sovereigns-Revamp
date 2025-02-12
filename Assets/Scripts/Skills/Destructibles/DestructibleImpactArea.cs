﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestructibleImpactArea : MonoBehaviour
{
    MeshRenderer[] _all;
    public Vector3[] rotAngles;
    CamRotationController _cam;
    public DestructiblePopupGraphic popup;
    public bool isShown;
    Collider _col;

    void Start()
    {
        EventManager.AddEventListener(GameEvents.GameStarted, OnBeginGame);
        EventManager.AddEventListener(GameEvents.EndMatch, OnEndMatch);
    }

    void OnBeginGame(object[] paramsContainer)
    {
        _all = GetComponentsInChildren<MeshRenderer>().Where(x => x.gameObject != gameObject).ToArray();
        rotAngles = GetComponentInParent<DestructibleObject>().rotAngles;
        _col = GetComponentInParent<Collider>();
        isShown = false;
        FindCamera();
        SetVisible(false);
    }

    void OnEndMatch(object[] paramsContainer)
    {
        EventManager.RemoveEventListener(GameEvents.GameStarted, OnBeginGame);
        EventManager.RemoveEventListener(GameEvents.EndMatch, OnEndMatch);
    }

    void FindCamera()
    {
        if (GameManager.screenDivided)
        {
            if (gameObject.layer == Utilities.IntLayers.VISIBLETOP1)
            {
                _cam = GameObject.Find("Player1").GetComponent<PlayerInput>().GetCamera;
                if (popup == null)
                {
                    popup = transform.parent.Find("PopupP1").GetComponentInParent<DestructiblePopupGraphic>();
                }
            }
            else if (gameObject.layer == Utilities.IntLayers.VISIBLETOP2)
            {
                _cam = GameObject.Find("Player2").GetComponent<PlayerInput>().GetCamera;
                if (popup == null)
                {
                    popup = transform.parent.Find("PopupP2").GetComponentInParent<DestructiblePopupGraphic>();
                }
            }
        }
        else
        {
            _cam = GameObject.FindObjectOfType<CamRotationController>();
            if (popup == null)
            {
                if (gameObject.layer == Utilities.IntLayers.VISIBLETOP1)
                {
                    popup = transform.parent.Find("PopupP1").GetComponentInParent<DestructiblePopupGraphic>();
                }
                else if (gameObject.layer == Utilities.IntLayers.VISIBLETOP2)
                {
                    popup = transform.parent.Find("PopupP2").GetComponentInParent<DestructiblePopupGraphic>();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (rotAngles.Length > 0)
        {
            if (!_cam.gameObject.activeInHierarchy) FindCamera();
            CheckRotation(_cam.transform.forward, _cam.AngleVision);
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

        var parent = GetComponentInParent<DestructibleObject>().transform;

        transform.rotation = Quaternion.Euler(vectorRot);
    }

    public void SetVisible(bool activate)
    {
        foreach (var m in _all)
        {
            m.enabled = activate;
        }
        isShown = activate;
        popup.Activate(_cam, _col, activate);
    }
}
