using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityLerper : MonoBehaviour
{
    public AnimationCurve curve;
    private Light lgt;
    private float lgtIntensity;
    void Start ()
    {
        lgt = GetComponent<Light>();
        lgtIntensity = lgt.intensity;
	}
	
	void Update ()
    {
        lgt.intensity = lgtIntensity * curve.Evaluate(Time.realtimeSinceStartup);
	}
}
