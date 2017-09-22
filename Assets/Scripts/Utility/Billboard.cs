using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Renderer _graph;
    Camera _cam;
    void Start()
    {
        _graph = GetComponent<Renderer>();
        _graph.enabled = false;
        EventManager.AddEventListener("LockOnActivated", OnLockOnActivation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramsContainer">
    /// 0 - Camera || 
    /// 1 - Sender name - string ||
    /// 2 - Activate - bool||
    /// 3 - Layer
    /// </param>
    void OnLockOnActivation(object[] paramsContainer)
    {
        _cam = (Camera)paramsContainer[0];
        var sender = (string)paramsContainer[1];
        var activate = (bool)paramsContainer[2];
        var layer = (int)paramsContainer[3];

        if (sender == GetComponentInParent<Player1Input>().gameObject.name) return;

        if (activate)
        {
            gameObject.layer = layer;
            _graph.enabled = true;
        }
        else
        {
            _graph.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
            _cam = null;
        }
    }

    void LateUpdate()
    {
        if (_graph.enabled && _cam != null)
        {
            transform.LookAt(_cam.transform.position);
        }
        
    }
}
