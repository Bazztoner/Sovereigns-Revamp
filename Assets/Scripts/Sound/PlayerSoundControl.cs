using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundControl : MonoBehaviour
{
    void OnSwordSoundStart()
    {
        EventManager.DispatchEvent("Sword Sound");
    }

    void OnShieldBashSound()
    {
        EventManager.DispatchEvent("Shield Bash Sound");
    }
}
