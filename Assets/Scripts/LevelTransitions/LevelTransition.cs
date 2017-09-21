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
    public LevelTransition otherSide;
    public int damage;
    public List<GameObject> playersInTrigger;
    public bool canBeUsed = true;
    public Camera[] camerasForTransition;

    void Start()
    {
        attackerTransitionOrigin = transform.Find("AttackerOrigin");
        victimTransitionOrigin = transform;
        attackerRelocatePoint = transform.Find("RelocationPoints").Find("AttackerRelocationPoint");
        victimRelocatePoint = transform.Find("RelocationPoints").Find("VictimRelocationPoint");
        canBeUsed = true;

        camerasForTransition = new Camera[2];
        var camContainer = transform.Find("Cameras");
        var cam1 = camContainer.Find("Camera1");
        var cam2 = camContainer.Find("Camera2");
        camerasForTransition[0] = cam1.GetComponent<Camera>();
        camerasForTransition[1] = cam2.GetComponent<Camera>();

        if (otherSide == this) throw new System.Exception("SOS PELOTUDO MAN PUSISTE UNO QUE SEA AMBOS LADOS: ERROR EN " + gameObject.name);
        //TODO: Agregar que encuentre por código el lado opuesto :v
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