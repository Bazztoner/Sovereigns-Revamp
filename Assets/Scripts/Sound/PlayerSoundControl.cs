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
        if (gameObject.name == (string)info[0])
        {
            if(gameObject.name == "Player2")EventManager.DispatchEvent(SoundEvents.DemonDamaged);
            else EventManager.DispatchEvent(SoundEvents.AngelDamaged);
        }
    }

    void OnStun(params object[] info)
    {
        if (gameObject.name == (string)info[0])
        {
            EventManager.DispatchEvent(SoundEvents.Parry);
        }
    }

    void OnPlayerDeath(object[] paramsContainer)
    {
        if (gameObject.name == (string)paramsContainer[0])
        {
            EventManager.DispatchEvent(SoundEvents.PlayerDeath);
        }
    }
}
