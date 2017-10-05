using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip angelAttack;
    public AudioClip angelShieldBash;
    public AudioClip angelBlock;
    public AudioClip stun;
    public AudioClip angelDamage;
    public AudioClip angelDeath;

    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EventManager.AddEventListener("Sword Sound", OnSwordAttack);
        EventManager.AddEventListener("Shield Bash Sound", OnShieldBash);
        EventManager.AddEventListener("Angel Block Sound", OnAngelBlock);
        EventManager.AddEventListener("Stun Sound", OnStun);
        EventManager.AddEventListener("Angel Damaged", OnAngelDamaged);
        EventManager.AddEventListener("Angel Death", OnAngelDeath);
    }

    void OnSwordAttack(params object[] info)
    {
        PlaySound(angelAttack);
    }

    void OnShieldBash(params object[] info)
    {
        PlaySound(angelShieldBash);
    }

    void OnAngelBlock(params object[] info)
    {
        PlaySound(angelBlock);
    }

    void OnStun(params object[] info)
    {
        PlaySound(stun);
    }

    void OnAngelDamaged(params object[] info)
    {
        PlaySound(angelDamage);
    }

    void OnAngelDeath(params object[] info)
    {
        PlaySound(angelDeath);
    }

    void PlaySound(AudioClip aud)
    {
        audioSource.pitch = Random.Range(0.75f, 1.5f);
        audioSource.PlayOneShot(aud);
    }
}
