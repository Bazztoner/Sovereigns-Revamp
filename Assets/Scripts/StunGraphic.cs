using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGraphic : MonoBehaviour
{
    public float rotationSpeed;
    public float radius;

    public Transform center;
    public float angle;

    private void Start()
    {
        if (center == null)
        {
            center = transform.parent.parent;
        }
    }

    private void Update()
    {
        angle += rotationSpeed * Time.deltaTime;

        var offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;
        transform.position = center.position + offset;
        transform.position = new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z);
    }
}
