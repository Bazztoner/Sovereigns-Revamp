using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlinkSpots : MonoBehaviour
{
    public List<BlinkSpot> blinkSpots;

	void Start ()
    {
        foreach (BlinkSpot bs in GetComponentsInChildren<BlinkSpot>())
        {
            AddSpot(bs);
        }
	}

    public void RemoveSpot(BlinkSpot spot)
    {
        blinkSpots.Remove(spot);
    }

    public void AddSpot(BlinkSpot spot)
    {
        blinkSpots.Add(spot);
    }
}
