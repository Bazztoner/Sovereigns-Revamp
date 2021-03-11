using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerParticles : MonoBehaviour
{
    private Transform _skillPos;
    GameObject _runParticle;
    GameObject _blinkTrail;

    PlayerInput _playerInput;

    void Start()
    {
        Initialize();
    }


    void Update()
    {
        if (_runParticle.activeInHierarchy)
        {
            _runParticle.transform.forward = new Vector3(transform.TransformDirection(_playerInput.RunDirection).x, 0, transform.TransformDirection(_playerInput.RunDirection).z);
        }
    }

    #region Initialization
    private void Initialize()
    {
        _skillPos = transform.Find("SpellPos");

        AddEvents();
        AddRunParticle();
        AddBlinkTrail();
        _playerInput = GetComponent<PlayerInput>();
    }

    void AddEvents()
    {
        EventManager.AddEventListener(ParticleEvents.ActivateRunParticle, OnActivateRunning);
        EventManager.AddEventListener(GameEvents.GameFinished, OnGameFinished);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
    }

    void OnGameFinished(object[] paramsContainer)
    {
        EventManager.RemoveEventListener(ParticleEvents.ActivateRunParticle, OnActivateRunning);
        _blinkTrail.SetActive(false);
        _runParticle.SetActive(false);
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener(ParticleEvents.ActivateRunParticle, OnActivateRunning);
            EventManager.RemoveEventListener(GameEvents.GameFinished, OnGameFinished);
            EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);
        }
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
    #region RPC
    public GameObject RpcParticleCaller(string part, Vector3 pos)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    public GameObject RpcParticleCaller(string part, Vector3 pos, float lifeTime)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, lifeTime);
        return inst;
    }
    #endregion
    #region Part + pos/parent
    public GameObject ParticleCaller(GameObject part, Vector3 pos)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
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
    #endregion
    #region Part + pos/parent + forward
    public GameObject ParticleCaller(GameObject part, Vector3 pos, Vector3 fwd)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        inst.transform.forward = fwd;
        Destroy(inst, 3);
        return inst;
    }

    public GameObject ParticleCaller(string part, Vector3 pos, Vector3 fwd)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        inst.transform.forward = fwd;
        Destroy(inst, 3);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent, Vector3 fwd)
    {
        var inst = Instantiate(part, parent);
        inst.transform.forward = fwd;
        Destroy(inst, 3);
        return inst;
    }
    #endregion

    #region Part + pos/parent + lifetime
    public GameObject ParticleCaller(GameObject part, Vector3 pos, float lifeTime)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        Destroy(inst, lifeTime);
        return inst;
    }

    public GameObject ParticleCaller(string part, Vector3 pos, float lifeTime)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, lifeTime);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent, float lifeTime, bool alignToZero)
    {
        var inst = Instantiate(part, parent);
        if (alignToZero) inst.transform.localPosition = Vector3.zero;
        Destroy(inst, lifeTime);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, RectTransform parent, float lifeTime, bool alignToZero)
    {
        //parent.anchoredPosition = Vector2.zero;
        var inst = Instantiate(part, parent);
        var coso = Camera.main.WorldToScreenPoint(parent.position);
        if (alignToZero)
        {
            inst.transform.localPosition = coso;
            inst.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        Destroy(inst, lifeTime);
        return inst;
    }
    #endregion
    #region Part + pos/parent + lifetime + forward
    public GameObject ParticleCaller(GameObject part, Vector3 pos, float lifeTime, Vector3 fwd)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        inst.transform.forward = fwd;
        Destroy(inst, lifeTime);
        return inst;
    }

    public GameObject ParticleCaller(string part, Vector3 pos, float lifeTime, Vector3 fwd)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        inst.transform.forward = fwd;
        Destroy(inst, lifeTime);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent, float lifeTime, Vector3 fwd)
    {
        var inst = Instantiate(part, parent);
        inst.transform.forward = fwd;
        Destroy(inst, lifeTime);
        return inst;
    }
    #endregion

    public ParticleSystem ParticleDestroyer(string name)
    {
        var p = _skillPos.Find(name).GetComponent<ParticleSystem>();
        p.Stop();

        Destroy(p.gameObject, 3f);
        return p;
    }

    void OnActivateRunning(object[] paramsContainer)
    {
        if (GameManager.screenDivided)
        {
            if (transform.GetComponent<PlayerInput>().gameObject.name == (string)paramsContainer[0])
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
            if (transform.GetComponent<PlayerInput>().gameObject.name == (string)paramsContainer[0])
            {
                _blinkTrail.SetActive(true);
                StartCoroutine(DeactivateBlinkTrail(1f));
            }
        }
        else _blinkTrail.SetActive(true);
    }

    IEnumerator DeactivateBlinkTrail(float time)
    {
        yield return new WaitForSeconds(time);
        _blinkTrail.SetActive(false);
    }

    #endregion
}
