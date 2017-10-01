using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePopupGraphic : MonoBehaviour
{
    Collider _col;
    DestructibleImpactArea _impactArea;
    DestructibleObject _obj;
    CamRotationController _cam;

    void Start()
    {
        EventManager.AddEventListener("BeginGame", OnBeginGame);
    }

    void OnBeginGame(object[] paramsContainer)
    {
        _col = GetComponentInParent<Collider>();
        _impactArea = GetComponentInParent<DestructibleImpactArea>();
        _obj = GetComponentInParent<DestructibleObject>();

        if (GameManager.screenDivided)
        {
            if (gameObject.layer == Utilities.IntLayers.VISIBLETOP1)
            {
                _cam = GameObject.Find("Player1").GetComponent<Player1Input>().GetCamera;
            }
            else if (gameObject.layer == Utilities.IntLayers.VISIBLETOP2)
            {
                _cam = GameObject.Find("Player2").GetComponent<Player1Input>().GetCamera;
            }
        }
        else _cam = GameObject.FindObjectOfType<CamRotationController>();

    }

    void LateUpdate()
    {
        if(_cam != null)
        {
            transform.LookAt(_cam.transform.position);
            transform.position = _col.ClosestPointOnBounds(new Vector3(_cam.transform.position.x, 0, _cam.transform.position.z));
        }
    }
}
