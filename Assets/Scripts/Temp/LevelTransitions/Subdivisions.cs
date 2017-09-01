using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subdivisions : MonoBehaviour
{
    public bool useGraphics = true;

    void Start ()
    {
        if (useGraphics)
        {
            var zones = GetComponentsInChildren<Transform>();
            foreach (Transform z in zones)
            {
                var r = z.GetComponent<Renderer>();
                if (r != null )
                {
                    r.enabled = true;
                }
               
            }
        }
        if (!useGraphics)
        {
            var zones = GetComponentsInChildren<Transform>();
            foreach (Transform z in zones)
            {
                var r = z.GetComponent<Renderer>();
                if (r != null)
                {
                    r.enabled = false;
                }

            }
        }
    }
	
	void Update ()
    {
    }
}
