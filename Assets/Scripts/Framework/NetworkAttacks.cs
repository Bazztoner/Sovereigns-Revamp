using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAttacks : Photon.MonoBehaviour {

    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    [PunRPC]
    public void SetX()
    {
        if(_anim != null) _anim.SetBool("X", true);
    }

    [PunRPC]
    public void SetY()
    {
        if(_anim != null) _anim.SetBool("Y", true);
    }

    [PunRPC]
    public void SetRollingOn()
    {
        if (_anim != null)
            _anim.SetBool("isRolling", true);
    }

    [PunRPC]
    public void SetRollingOff()
    {
        if (_anim != null)
            _anim.SetBool("isRolling", false);
    }
	
	[PunRPC]
    public void SetDamageOn()
    {
        if (_anim != null)
            _anim.SetBool("isDamaged", true);
    }

    [PunRPC]
    public void SetDeathOn()
    {
        if (_anim != null)
            _anim.SetBool("isDead", true);
    }

    [PunRPC]
    public void SetDamageOff()
    {
        if (_anim != null)
            _anim.SetBool("isDamaged", false);
    }
}
