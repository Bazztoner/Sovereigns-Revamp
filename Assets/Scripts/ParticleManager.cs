using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    //Primitivo, cambiar a Pool
    // ESTO AHORA SE VA A USAR CON GAMEOBJECTS, TENEMOS PARTÍCULAS DE MÁS DE UN SISTEMA
    public GameObject[] parts;

    void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;
    }

    void Start ()
    {
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamage);
        EventManager.AddEventListener("StunParticle", OnStunParticle);
        EventManager.AddEventListener("GuardBreakParticle", OnGuardBreakParticle);
        EventManager.AddEventListener("BlockParticle", OnBlockParticle);
        EventManager.AddEventListener("EnemyDamaged", OnEnemyDamage);
        EventManager.AddEventListener("TelekinesisObjectPulled", OnObjectPulled);
        //EventManager.AddEventListener("TelekinesisObjectLaunched", OnObjectLaunched);
        EventManager.AddEventListener("RepulsiveTelekinesisLoad", OnRepulsiveTelekinesisLoad);
        EventManager.AddEventListener("RepulsiveTelekinesisCasted", OnRepulsiveTelekinesisCasted);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramsContainer">
    /// CasterName - String ||  
    /// Position ||  
    /// PlayerParticles ||  
    /// _stunTime
    /// </param>
    void OnStunParticle(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var allChilds = caster.GetComponentsInChildren<Transform>();
        var stunTime = (float)paramsContainer[3];
        Transform parent = caster.transform;

        foreach (Transform child in allChilds)
        {
            if (child.name == "Helmet")
            {
                parent = child;
                break;
            }
        }

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "StunGraphic", parent, stunTime);
        else caster.ParticleCaller(parts[(int)ParticleID.StunGraphic].gameObject, parent, stunTime, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramsContainer">
    /// CasterName - String ||  
    /// Position ||  
    /// PlayerParticles ||  
    /// _breakTime
    /// </param>
    void OnGuardBreakParticle(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var allChilds = caster.GetComponentsInChildren<Transform>();
        var stunTime = (float)paramsContainer[3];
        Transform parent = caster.transform;

        foreach (Transform child in allChilds)
        {
            if (child.name == "Shield")
            {
                parent = child;
                break;
            }
        }

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "GuardBreakGraphic", parent, stunTime);
        else caster.ParticleCaller(parts[(int)ParticleID.GuardBreakGraphic].gameObject, parent, stunTime, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="paramsContainer">
    /// CasterName - String ||  
    /// Position ||  
    /// PlayerParticles ||  
    /// </param>
    void OnBlockParticle(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var allChilds = caster.GetComponentsInChildren<Transform>();
        Vector3 pos = Vector3.zero;

        foreach (Transform child in allChilds)
        {
            if (child.name == "Shield")
            {
                pos = child.position;
                break;
            }
        }

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "BlockingSparks", pos, caster.transform.forward);
        else caster.ParticleCaller(parts[(int)ParticleID.BlockingSparks].gameObject, pos, caster.transform.forward);
    }

    void OnRepulsiveTelekinesisLoad(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var tempPos = (Vector3)paramsContainer[0];
        var pos = new Vector3(tempPos.x, tempPos.y - 1, tempPos.z);

        if(!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Charge", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.ChargeShockwave].gameObject, pos);
    }

    void OnRepulsiveTelekinesisCasted(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var pos = (Vector3)paramsContainer[0];

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "ShockWave", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.LaunchShockwave].gameObject, pos);
    }

    void OnCharacterDamage(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var pos = new Vector3(tempPos.x, tempPos.y + 0.66f, tempPos.z);

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Blood", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.PlayerEntityDamage].gameObject, pos);
    }

    void OnEnemyDamage(object[] paramsContainer)
    {
        var caster = (Enemy)paramsContainer[1];
        var tempPos = (Vector3)paramsContainer[0];
        var pos = new Vector3(tempPos.x, tempPos.y + 0.66f, tempPos.z);

        caster.ParticleCaller(parts[(int)ParticleID.PlayerEntityDamage].gameObject, pos);
    }

    void OnObjectPulled(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var tempPos = (Transform)paramsContainer[0];
        
        var inst = caster.ParticleCaller(parts[(int)ParticleID.MagicAura].gameObject, tempPos);

        inst.transform.localPosition = Vector3.zero;
        inst.name = parts[(int)ParticleID.MagicAura].gameObject.name;
    }

    void OnObjectLaunched(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];

        var obj = caster.ParticleDestroyer(parts[(int)ParticleID.MagicAura].gameObject.name);
    }
}

public enum ParticleID
{
    PlayerEntityDamage,
    MagicAura,
    BlockingSparks,
    ChargeShockwave,
    LaunchShockwave,
    StunGraphic,
    GuardBreakGraphic,
    Count
}
