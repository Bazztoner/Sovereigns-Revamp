using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour {
    
    private Vector3 _realPosition = Vector3.zero;
    private Quaternion _realRotation = Quaternion.identity;
    private Animator _anim;
    private bool _isFirstConection;

	void Start ()
    {
        _anim = GetComponent<Animator>();
        _isFirstConection = true;
	}
	
	void Update ()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, _realPosition, 0.2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _realRotation, 0.2f);
        }
	}

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if (_anim != null)
            {
                stream.SendNext(_anim.GetBool("runForward"));
                stream.SendNext(_anim.GetBool("runLeft"));
                stream.SendNext(_anim.GetBool("runRight"));
                stream.SendNext(_anim.GetFloat("speedMult"));
                stream.SendNext(_anim.GetBool("isBlocking"));
            }
        }
        else
        {
            _realPosition = (Vector3)stream.ReceiveNext();
            _realRotation = (Quaternion)stream.ReceiveNext();
            if (_anim != null)
            {
                _anim.SetBool("runForward", (bool)stream.ReceiveNext());
                _anim.SetBool("runLeft", (bool)stream.ReceiveNext());
                _anim.SetBool("runRight", (bool)stream.ReceiveNext());
                _anim.SetFloat("speedMult", (float)stream.ReceiveNext());
                _anim.SetBool("isBlocking", (bool)stream.ReceiveNext());
            }
            
            if (_isFirstConection)
            {
                _isFirstConection = false;
                transform.position = _realPosition;
                transform.rotation = _realRotation;
            }
        }
    }
}
