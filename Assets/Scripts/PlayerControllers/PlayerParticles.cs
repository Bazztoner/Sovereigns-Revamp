using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : Photon.MonoBehaviour {

    private Transform _skillPos;

    //REVISAR: Estas dos variables las puse porque estaban en el characterMovement, pero parece ser que no se usan en ningun lado.
    public ParticleSystem launchParticleSystem;
    public ParticleSystem chargeParticleSystem;

    void Start()
    {
        Initialize();
    }

    #region Initialization
    private void Initialize()
    {
        _skillPos = transform.Find("SpellPos");
        launchParticleSystem = transform.Find("Launch").GetComponent<ParticleSystem>();
        chargeParticleSystem = transform.Find("Charge").GetComponent<ParticleSystem>();

        launchParticleSystem.transform.position = this.transform.position;
        chargeParticleSystem.transform.position = this.transform.position;
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
    #endregion
}
