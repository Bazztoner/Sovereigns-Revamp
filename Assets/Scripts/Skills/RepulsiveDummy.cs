using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RepulsiveDummy : MonoBehaviour
{
    RaycastHit rch;

    public void Execute(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        RepelObjects(skillPos, castTime, radialRange, verticalForce, radialForce, layerMask);
        Destroy(gameObject, 0.3f);
    }

    void RepelObjects(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        var _telekObjs = TelekineticObject.allObjs;
        var objs = new List<TelekineticObject>();

        foreach (var item in _telekObjs)
        {
            if (Vector3.Distance(skillPos.position, item.transform.position) < radialRange && !item.IsGrabbed) objs.Add(item);
        }

        if (!objs.Any()) { return; }

        foreach (var o in objs)
        {
            Rigidbody rig = o.GetComponent<Rigidbody>();
            var inVisionRange = Physics.Raycast(skillPos.position, o.transform.position - skillPos.position, out rch, 100, layerMask);

            Debug.DrawRay(skillPos.position, o.transform.position - skillPos.position, Color.red, 1);

            o.ChangeState(PhotonNetwork.player.NickName);
            rig.AddForce(Vector3.up * verticalForce);
            rig.AddExplosionForce(radialForce, skillPos.transform.position, radialRange);
            o.RepelObject();                                             
        }
    }
}
