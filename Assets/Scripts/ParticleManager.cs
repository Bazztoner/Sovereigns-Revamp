using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    //Primitivo, cambiar a Pool
    // ESTO AHORA SE VA A USAR CON GAMEOBJECTS, TENEMOS PARTÍCULAS DE MÁS DE UN SISTEMA
    /// <summary>
    /// HACE COMO 4 MESES QUE DIJE QUE LO IBA A CAMBIAR A POOL Y NO LO HICE
    /// </summary>
    public GameObject[] parts;

    void Awake()
    {
        if (instance != null) Destroy(instance);
        else instance = this;
    }

    void Start()
    {
        EventManager.AddEventListener(CharacterEvents.CharacterDamaged, OnCharacterDamaged);
        EventManager.AddEventListener(ParticleEvents.StunParticle, OnStunParticle);
        EventManager.AddEventListener(ParticleEvents.GuardBreakParticle, OnGuardBreakParticle);
        EventManager.AddEventListener(ParticleEvents.BlockParticle, OnBlockParticle);
        EventManager.AddEventListener(ParticleEvents.ToxicDamageParticle, OnToxicDamageParticle);
        EventManager.AddEventListener(SkillEvents.SpellBeingCasted, OnSpellBeingCasted);
        EventManager.AddEventListener(SkillEvents.RepulsiveTelekinesisCasted, OnRepulsiveTelekinesisCasted);
    }

    void OnSpellChanged(object[] paramsContainer)
    {
        var sender = (string)paramsContainer[0];
        var prnt = (RectTransform)paramsContainer[1];

        var caster = GameObject.Find(sender).GetComponent<PlayerParticles>();

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "SpellChangeParticle", prnt, 1.1f);
        else caster.ParticleCaller(parts[(int)ParticleID.SpellChangeParticle].gameObject, prnt, 1.1f, true);
    }

    public static void DestroyInstance()
    {
        instance = null;
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

        var nupoz = new Vector3(parent.transform.position.x, parent.transform.position.y + 1, parent.transform.position.z);

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
        var breakTime = (float)paramsContainer[3];
        Transform parent = caster.transform;

        foreach (Transform child in allChilds)
        {
            if (caster.gameObject.name == "Player2")
            {
                if (child.name == "LeftPalm" || child.name == "RightPalm")
                {
                    parent = child;
                    if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "GuardBreakGraphic", parent, breakTime);
                    else caster.ParticleCaller(parts[(int)ParticleID.GuardBreakGraphic].gameObject, parent, breakTime, true);
                }
            }
            else
            {
                if (child.name == "Shield")
                {
                    parent = child;
                    if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "GuardBreakGraphic", parent, breakTime);
                    else caster.ParticleCaller(parts[(int)ParticleID.GuardBreakGraphic].gameObject, parent, breakTime, true);
                    break;
                }
            }
        }
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

    void OnToxicDamageParticle(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var pos = new Vector3(tempPos.x, tempPos.y + 0.66f, tempPos.z);

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Toxine", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.ToxineDamage].gameObject, pos);
    }

    void OnSpellBeingCasted(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var tempPos = (Vector3)paramsContainer[0];
        var pos = new Vector3(tempPos.x, tempPos.y - 1, tempPos.z);

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Charge", pos);
<<<<<<< HEAD
        else caster.ParticleCaller(parts[(int)ParticleID.ChargeShockwave].gameObject, pos);
=======
        else caster.ParticleCaller(parts[(int)particleID].gameObject, pos);
    }

    void OnBlinkCasted(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var sender = caster.gameObject.name;
        var pos = (Vector3)paramsContainer[0];
        //var pos = new Vector3(tempPos.x, tempPos.y - 1, tempPos.z);

        var partID = sender == "Player1" ? ParticleID.AngelBlink : ParticleID.DemonBlink;

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Blink", pos);
        else
        {
            caster.ParticleCaller(parts[(int)partID].gameObject, caster.transform);
            caster.ParticleCaller(parts[(int)ParticleID.ResidualBlink].gameObject, pos);
        }
>>>>>>> Arreglado bug de que AtractiveTelekinesis no funcionaba.
    }

    void OnRepulsiveTelekinesisCasted(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[1];
        var pos = (Vector3)paramsContainer[0];

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "ShockWave", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.LaunchShockwave].gameObject, pos);
    }

    void OnCharacterDamaged(object[] paramsContainer)
    {
        var caster = (PlayerParticles)paramsContainer[2];
        var tempPos = (Vector3)paramsContainer[1];
        var pos = new Vector3(tempPos.x, tempPos.y + 0.66f, tempPos.z);

        if (!PhotonNetwork.offlineMode) caster.photonView.RPC("RpcParticleCaller", PhotonTargets.All, "Blood", pos);
        else caster.ParticleCaller(parts[(int)ParticleID.PlayerEntityDamage].gameObject, pos);
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
    ToxineDamage,
    SpellChangeParticle,
<<<<<<< HEAD
=======
    ToxicSpit,
    ToxineBubbles,
    ScalesCharge,
    BerserkCharge,
    AngelBlink,
    DemonBlink,
    ResidualBlink,
>>>>>>> Arreglado bug de que AtractiveTelekinesis no funcionaba.
    Count
}
