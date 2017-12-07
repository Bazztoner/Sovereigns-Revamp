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
        EventManager.AddEventListener(SoundEvents.SwordSound, OnSwordAttack);
        EventManager.AddEventListener(SoundEvents.ShieldBashSound, OnShieldBash);
        EventManager.AddEventListener(SoundEvents.BlockSound, OnAngelBlock);
        EventManager.AddEventListener(SoundEvents.StunSound, OnStun);
        EventManager.AddEventListener(SoundEvents.PlayerDamagedSound, OnPlayerDamaged);
        EventManager.AddEventListener(SoundEvents.PlayerDeathSound, OnPlayerDeath);
    }

    void OnSwordAttack(params object[] info)
    {
        PlaySound(angelAttack, Random.Range(0.8f, 1.2f));
    }

    void OnShieldBash(params object[] info)
    {
        PlaySound(angelShieldBash, Random.Range(0.9f, 1.1f));
    }

    void OnAngelBlock(params object[] info)
    {
        PlaySound(angelBlock, Random.Range(0.9f, 1.1f));
    }

    void OnStun(params object[] info)
    {
        PlaySound(stun);
    }

    void OnPlayerDamaged(params object[] info)
    {
        PlaySound(angelDamage, Random.Range(0.95f, 1.1f));
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
