using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Photon.MonoBehaviour {

    private bool _gameInCourse;
    private bool _isBlocking = false;
    private float _regenMult = 0.16f;

    [HideInInspector]
	public bool isDamaged = false;
    [HideInInspector]
    public bool isDead = false;

    public float hp;
    public int maxHp;
    public float mana;
    public int maxMana;
    public float hpRegeneration;
    public float manaRegeneration;

    #region Get and Set
    public float Hp
    {
        get { return hp; }
        set
        {
            if (value > maxHp) hp = maxHp;
            else if (value <= 0)
            {
                hp = 0;
                CancelInvoke();
                if (!PhotonNetwork.offlineMode) EventManager.DispatchEvent("PlayerDeath", new object[] { PhotonNetwork.player.NickName });
                else if (GameManager.screenDivided) EventManager.DispatchEvent("PlayerDeath", new object[] { this.gameObject.name });
            }
            else hp = value;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if (value > maxMana) mana = maxMana;
            else if ((value < 0)) mana = 0;
            else mana = value;
        }
    }
    #endregion

    void Start()
    {
        AddEvents();

        InvokeRepeating("Regenerate", _regenMult, _regenMult);
    }

    #region Initialization
    
    //This is for multiplayer only
    public void Initialize()
    {
        hp = 500;
        maxHp = 500;
        mana = 215;
        maxMana = 215;
        hpRegeneration = 2;
        manaRegeneration = 5;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("SpellCasted", OnSpellCasted);
        EventManager.AddEventListener("GameFinished", OnGameFinished);
        EventManager.AddEventListener("Blocking", OnBlocking);
        EventManager.AddEventListener("PlayerDeath", OnPlayerDeath);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
    }
    #endregion

    #region Status Update
    void ConsumeMana(float cost)
    {
        Mana -= cost;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("ManaUpdate", new object[] { Mana, fill, this.gameObject.name });
    }

    void RegainMana(float regained)
    {
        Mana += regained;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("ManaUpdate", new object[] { Mana, fill, this.gameObject.name });
    }
    void LoseHP(float damage)
    {
        var dmg = _isBlocking ? damage / 3 : damage;
        float fill = (Hp - dmg) / maxHp;
        if (fill < 0) fill = 0;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, (dmg > Hp ? 0 : Hp - dmg), fill });
        Hp = dmg >= Hp ? 0 : Hp - dmg;
    }
    void RegainHp(float regained)
    {
        Hp += regained;
        float fill = Hp / maxHp;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, Hp, fill });
    }

    public void TakeDamage(float damage)
    {
        EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });
        LoseHP(damage);
    }

    private void Regenerate()
    {
        if (hp < maxHp || mana < maxMana)
        {
            RegainHp(hpRegeneration * _regenMult);
            RegainMana(manaRegeneration * _regenMult);
        }
    }
    #endregion

    #region Events
    void OnSpellCasted(params object[] paramsContainer)
    {
        if ((string)paramsContainer[1] == this.gameObject.name)
        {
            ConsumeMana((int)paramsContainer[0]);
        }
       
    }

    private void OnGameFinished(params object[] paramsContainer)
    {
        _gameInCourse = false;
    }

    private void OnBlocking(params object[] paramsContainer)
    {
        if((string)paramsContainer[0] == this.gameObject.name)
            _isBlocking = (bool)paramsContainer[1];
    }

    private void OnPlayerDeath(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            isDead = true;
            CancelInvoke();
            EventManager.DispatchEvent("IsDead", new object[] { this.gameObject.name, isDead });
        }

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetDeathOn", PhotonTargets.All);
    }

    private void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            isDamaged = true;
            EventManager.DispatchEvent("IsDamaged", new object[] { this.gameObject.name, isDamaged });

            if (!PhotonNetwork.offlineMode) photonView.RPC("SetDamageOn", PhotonTargets.All);
        }
    }

    private void OnDamageExit()
    {
        isDamaged = false;
        EventManager.DispatchEvent("IsDamaged", new object[] { this.gameObject.name, isDamaged });

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetDamageOff", PhotonTargets.All);
    }
    #endregion
}
