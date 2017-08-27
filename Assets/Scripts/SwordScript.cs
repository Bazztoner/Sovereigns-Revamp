using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordScript : MonoBehaviour {

    private int _hitBoxLayer = 10;
    private int _appliedDamage = 0;
    private bool _isDetecting = false;

    void Start()
    {
        EventManager.AddEventListener("AttackEnter", OnAttackEnter);
    }

    private void OnAttackEnter(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<Player1Input>().gameObject.name == (string)paramsContainer[0])
            {
                _isDetecting = true;
                _appliedDamage = (int)paramsContainer[1];
            }
        }
        else
        {
            _isDetecting = true;
            _appliedDamage = (int)paramsContainer[1];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDetecting && other.gameObject.layer == _hitBoxLayer)
        {
            var dmgMult = other.transform.GetComponent<HitBoxScript>();
            float damage = dmgMult != null ? dmgMult.damageMult * _appliedDamage : _appliedDamage;
            
            _isDetecting = false;
            _appliedDamage = 0;
            //EventManager.DispatchEvent("DamageMade", new object[] { damage });
            if (!PhotonNetwork.offlineMode) other.gameObject.GetComponentInParent<DataSync>().photonView.RPC("TakeDamage", PhotonTargets.All, damage, PhotonNetwork.player.NickName);
            else if (GameManager.screenDivided) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage);
            else if (other.gameObject.GetComponentInParent<Enemy>() != null) other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage);
        }
    }
}
