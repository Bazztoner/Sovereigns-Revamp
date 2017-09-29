using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RepulsiveDummy1 : MonoBehaviour
{
    RaycastHit rch;
    List<TelekineticObject> allObjs;

    public void Execute(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        StartCoroutine(CastTime(skillPos, castTime, radialRange, verticalForce, radialForce, layerMask));
    }

    void PullObjects(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        var _telekObjs = TelekineticObject.allObjs;
        allObjs = new List<TelekineticObject>();

        foreach (var item in _telekObjs)
        {
            if (Vector3.Distance(skillPos.position, item.transform.position) < radialRange && !item.IsGrabbed) allObjs.Add(item);
        }

        if (!allObjs.Any()) { return; }

        foreach (var o in allObjs)
        {
            var rndV = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
            o.PullObject(transform);
        }
    }

    void RepelObjects(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        if (!allObjs.Any()) return;

        foreach (var o in allObjs)
        {
            Rigidbody rig = o.GetComponent<Rigidbody>();
            var inVisionRange = Physics.Raycast(skillPos.position, o.transform.position - skillPos.position, out rch, 100, layerMask);

            Debug.DrawRay(skillPos.position, o.transform.position - skillPos.position, Color.red, 1);

            o.ChangeState(PhotonNetwork.player.NickName);

            o.LaunchObject();
            o.SetGrabbed(false);
            //o.RepelObject();                                             
        }
    }

    IEnumerator CastTime(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask)
    {
        PullObjects(skillPos, castTime, radialRange, verticalForce, radialForce, layerMask);
        yield return new WaitForSeconds(castTime);
        RepelObjects(skillPos, castTime, radialRange, verticalForce, radialForce, layerMask);
        Destroy(gameObject, 0.3f);
    }
}
