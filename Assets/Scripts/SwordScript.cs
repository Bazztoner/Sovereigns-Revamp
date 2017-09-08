using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordScript : MonoBehaviour
{
    private int _hitBoxLayer = 10;
    private int _appliedDamage = 0;
    private bool _isDetecting = false;
    TrailRenderer _trail;

    void Start()
    {
        EventManager.AddEventListener("AttackEnter", OnAttackEnter);
        EventManager.AddEventListener("AttackExit", OnAttackExit);
        GetTrail(false);
    }

    private void OnAttackEnter(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<Player1Input>().gameObject.name == (string)paramsContainer[0])
            {
                _isDetecting = true;
                _appliedDamage = (int)paramsContainer[1];
                ActivateTrail(true);
            }
        }
        else
        {
            _isDetecting = true;
            _appliedDamage = (int)paramsContainer[1];
            ActivateTrail(true);
        }
    }

    private void OnAttackExit(params object[] paramsContainer)
    {
        _isDetecting = false;
        _appliedDamage = 0;
        ActivateTrail(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDetecting && other.gameObject.layer == _hitBoxLayer)
        {
            var dmgMult = other.transform.GetComponent<HitBoxScript>();
            float damage = dmgMult != null ? dmgMult.damageMult * _appliedDamage : _appliedDamage;
            
            _isDetecting = false;
            _appliedDamage = 0;

            if (!PhotonNetwork.offlineMode)
            {
                other.gameObject.GetComponentInParent<DataSync>().photonView.RPC("TakeDamage", PhotonTargets.All, damage, PhotonNetwork.player.NickName, "Melee");
            }
            else if (GameManager.screenDivided)
            {
                other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "Melee");
            }
            else if (other.gameObject.GetComponentInParent<Enemy>() != null)
            {
                other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage, "Melee");
            }
        }
    }

    #region Cambios Iván 3/9
    void ActivateTrail(bool activate)
    {
        _trail.enabled = activate;
    }

    void GetTrail(bool isActiveFromStart)
    {
        var trailContainer = transform.parent.parent.Find("SwordTrail");
        if (trailContainer == null)
        {
            var tempTrail = GameObject.Instantiate(Resources.Load("SwordTrail") as GameObject, transform.parent.parent);
            tempTrail.transform.localPosition = new Vector3(-1.291f, 0, 0);
            _trail = tempTrail.GetComponent<TrailRenderer>();
            _trail.enabled = isActiveFromStart;
        }
        else
        {
            _trail = trailContainer.GetComponent<TrailRenderer>();
            _trail.enabled = isActiveFromStart;
        }
    }

    void GetTrail()
    {
        _trail = transform.parent.parent.Find("SwordTrail").GetComponent<TrailRenderer>();
    }
    #endregion
}
