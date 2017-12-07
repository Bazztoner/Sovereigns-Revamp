using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundControl : MonoBehaviour
{
    void Start()
    {
        EventManager.AddEventListener(CharacterEvents.CharacterDamaged, OnDamaged);
        EventManager.AddEventListener(CharacterEvents.Stun, OnStun);
        EventManager.AddEventListener(CharacterEvents.PlayerDeath, OnPlayerDeath);
    }
    
    void OnSwordSoundStart()
    {
        EventManager.DispatchEvent(SoundEvents.SwordSound);
    }

    void OnShieldBashSound()
    {
        EventManager.DispatchEvent(SoundEvents.ShieldBashSound);
    }

    void OnDamaged(params object[] info)
    {
        EventManager.DispatchEvent(SoundEvents.PlayerDamagedSound);
    }

    void OnStun(params object[] info)
    {
        EventManager.DispatchEvent(SoundEvents.StunSound);
    }

    void OnPlayerDeath(object[] paramsContainer)
    {
        EventManager.DispatchEvent(SoundEvents.PlayerDeathSound);
    }
}
