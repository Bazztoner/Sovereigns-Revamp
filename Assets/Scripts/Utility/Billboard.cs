using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    SpriteRenderer _graph;
    Camera _cam;
    void Start()
    {
        _graph = GetComponent<SpriteRenderer>();
        _graph.enabled = false;
        var pongCol = _graph.color;
        pongCol.r = 1;
        StartCoroutine(LerpColor(_graph.color, _graph.color, pongCol, 1.3f));
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
        var senderName = (string)paramsContainer[1];
        var activate = (bool)paramsContainer[2];
        var layer = (int)paramsContainer[3];
        var sender = GetComponentInParent<Player1Input>();

        if (sender != null)
        {
            if (senderName == sender.gameObject.name) return;
        }

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

    IEnumerator LerpColor(Color color, Color startValue, Color endValue, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            color = Color.Lerp(startValue, endValue, i);
            _graph.color = color;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PingPongColor(Color color, Color a, Color b, float maxTime)
    {
        var i = 0f;
        var reverse = false;

        while (true)
        {
            if (!reverse)
            {
                while (i <= 1)
                {
                    i += Time.deltaTime / maxTime;
                    color = Color.Lerp(a, b, i);
                    _graph.color = color;
                    yield return new WaitForEndOfFrame();
                }
                i = 0;
                reverse = !reverse;
            }
            else
            {
                while (i <= 1)
                {
                    i += Time.deltaTime / maxTime;
                    color = Color.Lerp(b, a, i);
                    _graph.color = color;
                    yield return new WaitForEndOfFrame();
                }
                i = 0;
                reverse = !reverse;
            }
        }
    }
}
