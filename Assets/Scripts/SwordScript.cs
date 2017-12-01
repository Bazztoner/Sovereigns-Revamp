using System;
using System.Linq;
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
    bool _isParry = false;
    bool _isGuardBreak = false;
    TrailRenderer _trail;
    bool _isActive;

    void Start()
    {
        GetTrail(false);
        AddEvents();
    }

    void AddEvents()
    {
        AddAttackEvents();
        AddAttackTypeEvents();
    }

    private void Update()
    {
       if(_isActive) print("Activate " + _isActive.ToString() + " " + gameObject.name);
    }

    void AddAttackEvents()
    {
        EventManager.AddEventListener("ActivateCollider", OnActivateCollider);
        EventManager.AddEventListener("AttackEnter", OnAttackEnter);
        EventManager.AddEventListener("AttackExit", OnAttackExit);
    }

    void OnActivateCollider(object[] paramsContainer)
    {
        if ((SwordScript)paramsContainer[0] == this)
        {
            var activate = (bool)paramsContainer[1];
            _isActive = activate;
            ActivateTrail(_isActive);
            print("Activate " + gameObject.name + activate.ToString());
        }
    }

    void AddAttackTypeEvents()
    {
        EventManager.AddEventListener("HorizontalAttack", OnHorizontalAttack);
        EventManager.AddEventListener("VerticalAttack", OnVerticalAttack);
        EventManager.AddEventListener("ParryAttack", OnParryAttack);
        EventManager.AddEventListener("GuardBreakAttack", OnGuardBreakAttack);
    }
    
    //Recive el evento que dispara el PlayerCombat desde el OnAttackEnter, y setea el daño que debe causar en el siguiente golpe que conecte, y activa el _isDetecting para saber que tiene
    //que empezar a detectar daño.
    //IVAN: aca vas a tener que hacer el cambio para cuando mandes el string, deberias chequear si el paramsContainer tiene longitud de 2 o 3, si es 3 viene el string que mandas vos.
    void OnAttackEnter(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<PlayerInput>().gameObject.name == (string)paramsContainer[0] && !_isDetecting && _isActive)
            {
                _isDetecting = true;
                _appliedDamage = (int)paramsContainer[1];
                ActivateTrail(true);
            }
        }
    }

    private void OnAttackExit(params object[] paramsContainer)
    {
        _isDetecting = false;
        _isHorizontal = false;
        _isVertical = false;
        ActivateTrail(false);
        _appliedDamage = 0;
    }

    private void OnHorizontalAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<PlayerMovement>().gameObject.name == (string)paramsContainer[0])
            {
                _isHorizontal = true;
                _isParry = false;
                _isVertical = false;
                _isGuardBreak = false;
            }
        }
        else
        {
            _isHorizontal = true;
            _isVertical = false;
            _isParry = false;
            _isGuardBreak = false;
        }
    }

    private void OnVerticalAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<PlayerMovement>().gameObject.name == (string)paramsContainer[0])
            {
                _isHorizontal = false;
                _isVertical = true;
                _isParry = false;
                _isGuardBreak = false;
            }
        }
        else
        {
            _isHorizontal = true;
            _isParry = false;
            _isVertical = false;
            _isGuardBreak = false;
        }
    }

    private void OnParryAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<PlayerMovement>().gameObject.name == (string)paramsContainer[0])
            {
                _isHorizontal = false;
                _isVertical = false;
                _isGuardBreak = false;
                _isParry = true;
            }
        }
        else
        {
            _isHorizontal = false;
            _isVertical = false;
            _isGuardBreak = false;
            _isParry = true;
        }
    }

    private void OnGuardBreakAttack(params object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (this.transform.GetComponentInParent<PlayerMovement>().gameObject.name == (string)paramsContainer[0])
            {
                _isHorizontal = false;
                _isVertical = false;
                _isGuardBreak = true;
                _isParry = false;
            }
        }
        else
        {
            _isHorizontal = false;
            _isVertical = false;
            _isGuardBreak = true;
            _isParry = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isDetecting && other.gameObject.layer == _hitBoxLayer && _isActive)
        {
            var dmgMult = other.transform.GetComponent<HitBoxScript>();
            float damage = dmgMult != null ? dmgMult.damageMult * _appliedDamage : _appliedDamage;
            var myName = this.GetComponentInParent<PlayerInput>().gameObject.name;
            var combo = this.GetComponentInParent<PlayerStats>().gameObject.name == "Player1" ? GameObject.Find("HUD1").GetComponent<HUDController>().Combo : 
                                                                                                GameObject.Find("HUD2").GetComponent<HUDController>().Combo;

            float mult = (combo / 10f + 1f);

            //Aplica al daño el multiplicador del combo. Ej: si el combo esta en 7, entonces 7 dividido 10 mas 1 da 1.7, y ese valor lo multiplica al daño.
            damage *= mult;

            _isDetecting = false;
            _appliedDamage = 0;

            if (!PhotonNetwork.offlineMode)
            {
                other.gameObject.GetComponentInParent<DataSync>().photonView.RPC("TakeDamage", PhotonTargets.All, damage, PhotonNetwork.player.NickName, "Melee");
            }
            else if (GameManager.screenDivided)
            {
                if (CheckIfFrontal(other))
                {
                    if (_isHorizontal) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "MeleeHorizontal", myName);
                    else if (_isVertical) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage / 3, "MeleeVertical", myName);
                    else if (_isParry) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "ParryAttack", myName);
                    else if (_isGuardBreak) other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage / 3, "GuardBreakAttack", myName);
                }
                else other.gameObject.GetComponentInParent<PlayerStats>().TakeDamage(damage, "Melee", this.GetComponentInParent<PlayerInput>().gameObject.name);

                //Esto es algo parecido a lo que hace con el daño. Lo que hace es que si el multiplicador esta en 15 por ejemplo, 
                //lo que va a ganar de mana es igual al daño que recibio multiplicado por 2.5
                other.gameObject.GetComponentInParent<PlayerStats>().RegainMana(damage / 2 * mult);
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
        //_trail.gameObject.SetActive(activate);
    }

    void GetTrail(bool isActiveFromStart)
    {
        var trailContainer = GetComponentInParent<PlayerInput>().transform.FindChild("SwordTrail");
        if (trailContainer == null)
        {
            var tempTrail = GameObject.Instantiate(Resources.Load("SwordTrail") as GameObject, transform.parent.parent);
            tempTrail.transform.localPosition = new Vector3(-1.291f, 0, 0);
            tempTrail.gameObject.name = "SwordTrail";
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
