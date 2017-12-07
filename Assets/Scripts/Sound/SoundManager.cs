using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip angelAttack;
    public AudioClip angelShieldBash;
    public AudioClip angelBlock;
    public AudioClip parry;
    public AudioClip angelDamaged;
    public AudioClip demonDamaged;
    public AudioClip angelDeath;

    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EventManager.AddEventListener(SoundEvents.SwordSound, OnSwordAttack);
        EventManager.AddEventListener(SoundEvents.ShieldBashSound, OnShieldBash);
        EventManager.AddEventListener(SoundEvents.AngelBlock, OnAngelBlock);
        EventManager.AddEventListener(SoundEvents.Parry, OnStun);
        EventManager.AddEventListener(SoundEvents.AngelDamaged, OnAngelDamaged);
        EventManager.AddEventListener(SoundEvents.DemonDamaged, OnDemonDamaged);
        EventManager.AddEventListener(SoundEvents.PlayerDeath, OnPlayerDeath);
    }

    void OnDemonDamaged(object[] paramsContainer)
    {
        PlaySound(demonDamaged, UnityEngine.Random.Range(0.8f, 1.2f));
    }

    void OnSwordAttack(params object[] info)
    {
        PlaySound(angelAttack, UnityEngine.Random.Range(0.8f, 1.2f));
    }

    void OnShieldBash(params object[] info)
    {
        PlaySound(angelShieldBash, UnityEngine.Random.Range(0.9f, 1.1f));
    }

    void OnAngelBlock(params object[] info)
    {
        PlaySound(angelBlock, UnityEngine.Random.Range(0.9f, 1.1f));
    }

    void OnStun(params object[] info)
    {
        PlaySound(parry, 0.85f);
    }

    void OnAngelDamaged(params object[] info)
    {
        PlaySound(angelDamaged, UnityEngine.Random.Range(0.95f, 1.1f));
    }

    void OnPlayerDeath(params object[] info)
    {
        PlaySound(angelDeath);
    }

    void PlaySound(AudioClip aud, float pitch = 1)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(aud);
    }
}
