using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGraphic : MonoBehaviour
{
    /*public Transform center;
    public float degreesPerSecond;

    Vector3 v;

    void Start()
    {
        v = transform.position - center.position;
    }

    void Update()
    {
        v = Quaternion.AngleAxis(degreesPerSecond * Time.unscaledDeltaTime, Vector3.up) * v;
        transform.position = center.position + v;
    }*/

    public float rotationSpeed;
    public float radius;

    public Transform center;
    public float angle;

    private void Start()
    {
        if(center == null) center = transform.parent.parent;
    }

    private void Update()
    {
        angle += rotationSpeed * Time.deltaTime;

        var offset = new Vector3(Mathf.Sin(angle), 0 , Mathf.Cos(angle)) * radius;
        transform.position = center.position + offset;
    }
}
