using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DMM_ArcaneRepulsion : MonoBehaviour
{
    RaycastHit _rch;
    int _damage = 65;

    public void Execute(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask, string caster)
    {
        RepelObjects(skillPos, castTime, radialRange, verticalForce, radialForce, layerMask, caster);
        Destroy(gameObject, 0.3f);
    }

    void RepelObjects(Transform skillPos, float castTime, float radialRange, float verticalForce, float radialForce, LayerMask layerMask, string caster)
    {
        var target = GameObject.FindObjectsOfType<Player1Input>().Where(x => x.gameObject.name != caster).FirstOrDefault();
        if (target == null) return;

        if (Vector3.Distance(skillPos.position, target.transform.position) < radialRange)
        {
            Rigidbody rig = target.GetComponent<Rigidbody>();
            var inVisionRange = Physics.Raycast(skillPos.position, target.transform.position - skillPos.position, out _rch, 100, layerMask);

            Debug.DrawRay(skillPos.position, target.transform.position - skillPos.position, Color.red, 1);

            rig.AddForce(Vector3.up * verticalForce);
            rig.AddExplosionForce(radialForce, skillPos.transform.position, radialRange);
            target.GetComponent<PlayerStats>().TakeDamage(_damage, "Spell", caster);
        }

        EventManager.DispatchEvent("RepulsiveTelekinesisCasted", new object[] { skillPos.position, skillPos.GetComponentInParent<PlayerParticles>() });
    }
}
