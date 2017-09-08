using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : Photon.MonoBehaviour {

    private Transform _skillPos;
    GameObject _runParticle;
    GameObject _blinkTrail;

    Player1Input _playerInput;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        /*if (-_playerInput.RunDirection != Vector3.zero)
        {
            _runParticle.transform.forward = -_playerInput.RunDirection;
        }*/
        
    }

    #region Initialization
    private void Initialize()
    {
        _skillPos = transform.Find("SpellPos");

        AddEvents();
        AddRunParticle();
        AddBlinkTrail();
        _playerInput = GetComponent<Player1Input>();
    }

    void AddEvents()
    {
        EventManager.AddEventListener("ActivateRunParticle", OnActivateRunning);
        //EventManager.AddEventListener("ActivateBlinkTrail", OnActivateBlink);
    }

    void AddRunParticle()
    {
        var tempRunParticle = transform.Find("RunningParticle");
        if (tempRunParticle == null)
        {
            var tempPart = GameObject.Instantiate(Resources.Load("Particles/RunningParticle") as GameObject, transform);
            tempPart.transform.localPosition = Vector3.zero;
            tempPart.transform.forward = tempPart.transform.parent.forward;
            _runParticle = tempPart.gameObject;
            _runParticle.SetActive(false);
        }
        else
        {
            _runParticle = tempRunParticle.gameObject;
            _runParticle.SetActive(false);
        }
    }

    void AddBlinkTrail()
    {
        var tempBlinkTrail = transform.Find("BlinkTrail");
        if (tempBlinkTrail == null)
        {
            var tempTrail = GameObject.Instantiate(Resources.Load("Spells/BlinkTrail") as GameObject, transform);
            tempTrail.transform.localPosition = Vector3.zero;
            tempTrail.transform.forward = tempTrail.transform.parent.forward;
            _blinkTrail = tempTrail.gameObject;
            _blinkTrail.SetActive(false);
        }
        else
        {
            _blinkTrail = tempBlinkTrail.gameObject;
            _blinkTrail.SetActive(false);
        }
    }
    #endregion

    #region Particles
    public GameObject ParticleCaller(GameObject part, Vector3 pos)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    [PunRPC]
    public GameObject RpcParticleCaller(string part, Vector3 pos)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }


    public GameObject ParticleCaller(string part, Vector3 pos)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent)
    {
        var inst = Instantiate(part, parent);
        Destroy(inst, 3);
        return inst;
    }

    public ParticleSystem ParticleDestroyer(string name)
    {
        var p = _skillPos.FindChild(name).GetComponent<ParticleSystem>();
        p.Stop();

        Destroy(p.gameObject, 3f);
        return p;
    }

    void OnActivateRunning(object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if(transform.GetComponent<Player1Input>().gameObject.name == (string)paramsContainer[0])
            {
                _runParticle.SetActive((bool)paramsContainer[1]);
            }
        }
        else _runParticle.SetActive((bool)paramsContainer[1]);
    }

    void OnActivateBlink(object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (transform.GetComponent<Player1Input>().gameObject.name == (string)paramsContainer[0])
            {
                _blinkTrail.SetActive(true);
                StartCoroutine(DeactivateBlinkTrail(1f));
            }
        }
        else
        {
            _blinkTrail.SetActive(true);
            StartCoroutine(DeactivateBlinkTrail(1f));
        }
    }

    IEnumerator DeactivateBlinkTrail(float time)
    {
        yield return new WaitForSeconds(time);
        _blinkTrail.SetActive(false);
    }

    #endregion
}
