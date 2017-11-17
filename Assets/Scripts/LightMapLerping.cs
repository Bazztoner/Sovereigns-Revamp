using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMapLerping : MonoBehaviour
{
    public AnimationCurve curve;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
    }

    void Update ()
    {
        mat.SetFloat("_alphaLerp", curve.Evaluate(Time.time));
	}
}
