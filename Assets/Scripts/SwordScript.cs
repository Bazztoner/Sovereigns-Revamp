using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordScript : MonoBehaviour
{
    private int _hitBoxLayer = 10;
    private int _appliedDamage = 0;
    private bool _isDetecting = false;
    private bool _isHorizontal = false;
    private bool _isVertical = false;
    TrailRenderer _trail;

    void Start()
    {
        EventManager.AddEventListener("AttackEnter", OnAttackEnter);
        EventManager.AddEventListener("AttackExit", OnAttackExit);
        EventManager.AddEventListener("HorizontalAttack", OnHorizontalAttack);
        EventManager.AddEventListener("VerticalAttack", OnVerticalAttack);
        GetTrail(false);
    }

    private void OnAttackEnter(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<Player1Input>().gameObject.name == (string)paramsContainer[0] && !_isDetecting)
            {
                _isDetecting = true;
                _appliedDamage = (int)paramsContainer[1];
                ActivateTrail(true);
            }
        }
        else if(!_isDetecting)
        {
            _isDetecting = true;
            _appliedDamage = (int)paramsContainer[1];
            ActivateTrail(true);
        }
    }

    private void OnAttackExit(params object[] paramsContainer)
    {
        _isDetecting = false;
        _isHorizontal = false;
        _isVertical = false;
        _appliedDamage = 0;
        ActivateTrail(false);
    }

    private void OnHorizontalAttack(params object[] paramsContainer)
    {
        if (this.transform.GetComponentInParent<Player1Input>().gameObject.name == (string)paramsContainer[0])
        {
            _isHorizontal = true;
            _isVertical = false;
        }
    }

    private void OnVerticalAttack(params object[] paramsContainer)
    {
        if (this.transform.GetComponentInParent<Player1Input>().gameObject.name == (string)paramsContainer[0])
        {
            _isHorizontal = false;
            _isVertical = true;
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

            //Test para la normal del polígono
            /*RaycastHit rch;
            var polyNormal = other.ClosestPointOnBounds(transform.position);

            if (Physics.Raycast(transform.position, other.transform.position - transform.position, out rch, 10))
            {
                //if pointing at the object you want to get collision point for
                if (rch.transform.gameObject == other.gameObject)
                {
                    //have locPos = the local position of the hit point
                    //polyNormal = transform.InverseTransformPoint(rch.point);
                    polyNormal = rch.point;
                }
            }*/

            if (!PhotonNetwork.offlineMode)
            {
                other.gameObject.GetComponentInParent<DataSync>().photonView.RPC("TakeDamage", PhotonTargets.All, damage, PhotonNetwork.player.NickName, "Melee");
            }
            else if (GameManager.screenDivided)
            {
                if (CheckIfFrontal(other))
                {
                    if(_isHorizontal) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "MeleeHorizontal");
                    else if(_isVertical) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "MeleeVertical");
                }
                else other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "Melee");
            }
            else if (other.gameObject.GetComponentInParent<Enemy>() != null)
            {
                other.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage, "Melee");
            }
        }
    }

    private bool CheckIfFrontal(Collider other)
    {
        var enemyForward = other.gameObject.GetComponentInParent<PlayerStats>().transform.forward;
        var myForward = GetComponentInParent<PlayerStats>().transform.forward;
        return Vector3.Angle(-enemyForward, myForward) <= 30 ? true : false;
    }

    void ActivateTrail(bool activate)
    {
        _trail.gameObject.SetActive(activate);
    }

    void GetTrail(bool isActiveFromStart)
    {
        var trailContainer = transform.parent.parent.Find("SwordTrail");
        if (trailContainer == null)
        {
            var tempTrail = GameObject.Instantiate(Resources.Load("SwordTrail") as GameObject, transform.parent.parent);
            tempTrail.transform.localPosition = new Vector3(-1.291f, 0, 0);
            _trail = tempTrail.GetComponent<TrailRenderer>();
            _trail.gameObject.SetActive(isActiveFromStart);
        }
        else
        {
            _trail = trailContainer.GetComponent<TrailRenderer>();
            _trail.gameObject.SetActive(isActiveFromStart);
        }
    }

    void GetTrail()
    {
        _trail = transform.parent.parent.Find("SwordTrail").GetComponent<TrailRenderer>();
    }
}
