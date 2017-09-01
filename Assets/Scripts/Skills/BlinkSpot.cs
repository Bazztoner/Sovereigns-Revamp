using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSpot : MonoBehaviour
{
    PlayerBlinkSpots mySpots;

    void Start()
    {
        mySpots = GetComponentInParent<PlayerBlinkSpots>();
    }

    void OnTriggerEnter(Collider t)
    {
        if (t.gameObject.layer != 10)
        {
            if (mySpots == null)
            {
                mySpots = GetComponentInParent<PlayerBlinkSpots>();
            }

            mySpots.RemoveSpot(this);
        }
        
    }
    void OnTriggerExit(Collider t)
    {
        if (t.gameObject.layer != 10)
        {
            if (mySpots == null)
            {
                mySpots = GetComponentInParent<PlayerBlinkSpots>();
            }

            mySpots.AddSpot(this);
        }
    }
}
