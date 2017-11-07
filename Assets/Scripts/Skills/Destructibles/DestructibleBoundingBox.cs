using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBoundingBox : MonoBehaviour
{
    private List<DataSync> _targets = new List<DataSync>();

    void OnTriggerEnter(Collider c)
    {
        var destr = GetComponentInParent<DestructibleObject>();

        if (c.gameObject.layer == 8)
        {
            //Hacer eventos para enviar daño, no manosear TakeDamage
            var dmg = transform.GetComponentInParent<DestructibleObject>().damage;
            c.GetComponent<Enemy>().TakeDamage(dmg, "Destructible");
            gameObject.SetActive(false);
        }
        else if (c.gameObject.layer == 13)
        {
            //Hacer eventos para enviar daño, no manosear TakeDamage
            var dmg = transform.GetComponentInParent<DestructibleObject>().damage;
            if (PhotonNetwork.offlineMode)
            {
                c.GetComponent<PlayerStats>().TakeDamage(dmg, "Destructible", gameObject.name);
                gameObject.SetActive(false);
            }
            else if (destr.nickName == PhotonNetwork.player.NickName)
            {
                var charac = c.GetComponent<DataSync>();

                foreach (var player in _targets)
                {
                    if (player.photonView.GetInstanceID() == charac.photonView.GetInstanceID())
                        return;
                }

                _targets.Add(charac);
                charac.photonView.RPC("TakeDamage", PhotonTargets.All, dmg, PhotonNetwork.player.NickName, "Destructible");
                gameObject.SetActive(false);
            } 
        }
    }
}
