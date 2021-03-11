using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBoundingBox : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        var destr = GetComponentInParent<DestructibleObject>();

        if (c.gameObject.layer == 8)
        {
            //Hacer eventos para enviar daño, no manosear TakeDamage
            var dmg = transform.GetComponentInParent<DestructibleObject>().damage;
            gameObject.SetActive(false);
        }
    }
}
