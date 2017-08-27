using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestructibleImpactArea : MonoBehaviour
{
    MeshRenderer[] all;
    public Vector3[] rotAngles;

    void Start()
    {
        all = GetComponentsInChildren<MeshRenderer>().Where(x => x.gameObject != gameObject).ToArray();
        rotAngles = GetComponentInParent<DestructibleObject>().rotAngles;
    }

    void LateUpdate()
    {
        if (rotAngles.Length != 0)
        {
            var cam = GameObject.FindObjectOfType<CamRotationController>();
            CheckRotation(cam.transform.forward, cam.AngleVision);
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
    }

    public void SetVisible(bool activate)
    {
        foreach (var m in all)
        {
            m.enabled = activate;
        }
    }
}
