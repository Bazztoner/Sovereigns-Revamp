using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GravitationalDummy1 : MonoBehaviour
{
    public Transform[] childs;

    void Awake()
    {
       childs = transform.GetComponentsInChildren<Transform>().Where(x => x.name != gameObject.name).ToArray();
    }

    public void Init(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void Execute(TelekineticObject[] objs, bool hasObject)
    {
        if (hasObject) PullObject(objs);
        else LaunchObject(objs, hasObject);
    }

    void PullObject(TelekineticObject[] objs)
    {
        var indx = 0;
        foreach (var o in objs)
        {
            o.PullObject(childs[indx]);
            indx++;
        }
    }

    void LaunchObject(TelekineticObject[] objs, bool hasObject)
    {
        foreach (var o in objs)
        {
            o.LaunchObject();
            o.SetGrabbed(hasObject);
        }
    }
}
