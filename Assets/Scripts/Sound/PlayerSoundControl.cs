using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundControl : MonoBehaviour
{
    void Start()
    {
        EventManager.AddEventListener("CharacterDamaged", OnDamaged);
        EventManager.AddEventListener("Stun", OnStun);
        EventManager.AddEventListener("PlayerDeath", OnAngelDeath);
    }
    
    void OnSwordSoundStart()
    {
        EventManager.DispatchEvent("Sword Sound");
    }

    void OnShieldBashSound()
    {
        EventManager.DispatchEvent("Shield Bash Sound");
    }

    void OnDamaged(params object[] info)
    {
        EventManager.DispatchEvent("Angel Damaged");
    }

    void OnStun(params object[] info)
    {
        EventManager.DispatchEvent("Stun Sound");
    }

    void OnAngelDeath(object[] paramsContainer)
    {
        EventManager.DispatchEvent("Angel Death");
    }
}
