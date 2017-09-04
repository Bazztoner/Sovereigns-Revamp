using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    public Zone from;
    public Zone to;
    public Transform attackerRelocatePoint;
    public Transform victimRelocatePoint;
    public Transform attackerTransitionOrigin;
    public Transform victimTransitionOrigin;
    public int damage;
    public List<GameObject> playersInTrigger;
    public bool canBeUsed = true;
    public Camera[] camerasForTransition;

    void Start()
    {
        attackerTransitionOrigin = transform.Find("AttackerOrigin");
        victimTransitionOrigin = transform;
        canBeUsed = true;
        var camContainer = transform.Find("Cameras");
        camerasForTransition = new Camera[2];
        camerasForTransition[0] = camContainer.Find("Camera1").GetComponent<Camera>();
        camerasForTransition[1] = camContainer.Find("Camera2").GetComponent<Camera>();
    }

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