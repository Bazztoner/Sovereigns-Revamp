using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalDummy : MonoBehaviour
{
    public void Init(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void Execute(TelekineticObject obj, bool hasObject)
    {
        if (hasObject) PullObject(obj);
        else LaunchObject(obj, hasObject);
    }

    void PullObject(TelekineticObject obj)
    {
        obj.PullObject(transform);
    }

    void LaunchObject(TelekineticObject obj, bool hasObject)
    {
        obj.LaunchObject();
        obj.SetGrabbed(hasObject);
    }
}
