using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    public Zone from;
    public Zone to;
    public Transform attackerRelocatePoint;
    public Transform attackedRelocatePoint;
    public int damage;
    public List<GameObject> playersInTrigger;

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == Utilities.IntLayers.PLAYER || c.gameObject.layer == Utilities.IntLayers.ENEMY)
        {
            playersInTrigger.Add(c.gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.layer == Utilities.IntLayers.PLAYER || c.gameObject.layer == Utilities.IntLayers.ENEMY)
        {
            playersInTrigger.Remove(c.gameObject);
        }
    }
}

/* TODO:
 * Que la transición se active si el atacante está mirando hacia el forward del trigger
 
*/