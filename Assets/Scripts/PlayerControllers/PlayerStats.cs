using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Photon.MonoBehaviour {

    private bool _gameInCourse;
    private bool _isBlocking = false;
    private bool _isBlockingUp = false;
    private bool _isKnockBackAttack = false;
    private bool _isStunAttack = false;
    private float _regenMult = 0.16f;
    private float _perfectBlockPerc = 10f;
    private float _imperfectBlockPerc = 45f;
    private float _stunTime = 1.2f;

    [HideInInspector]
	public bool isDamaged = false;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool uncancelableAttack = false;

    public float hp;
    public int maxHp;
    public float mana;
    public int maxMana;
    public float hpRegeneration;
    public float manaRegeneration;
    public Vector3 initialPosition;
    public Quaternion initialRotation;

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
                if (!PhotonNetwork.offlineMode && !isDead) EventManager.DispatchEvent("PlayerDeath", new object[] { PhotonNetwork.player.NickName });
                else if (GameManager.screenDivided && !isDead) EventManager.DispatchEvent("PlayerDeath", new object[] { this.gameObject.name });
                isDead = true;
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
        _gameInCourse = true;
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
        EventManager.AddEventListener("KnockBackEnter", OnKnockBackEnter);
        EventManager.AddEventListener("KnockBackExit", OnKnockBackExit);
        EventManager.AddEventListener("SpecialAttack", OnSpecialAttackUpdate);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
        //EventManager.AddEventListener("StunAttackEnter", OnStunAttackEnter);
        //EventManager.AddEventListener("StunAttackExit", OnStunAttackExit);
    }

    public void ApplyPlayerStartingColor(AngelArmorColor color)
    {
        color.transform.SetParent(transform);
        color.gameObject.SetActive(true);
        color.ExecuteColorizer();
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

    [System.Obsolete("Usar la que pide nombre de jugador")]
    void LoseHP(float damage, string attackType)
    {
        float dmg;
        bool blocked = true;

        if (attackType == "MeleeHorizontal")
        {
            if (_isBlockingUp)
            {
                dmg = (_imperfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("BlockSound");
                if (_isKnockBackAttack) EventManager.DispatchEvent("DoKnockBack", new object[] { this.gameObject.name });
            }
            else if (_isBlocking)
            {
                dmg = (_perfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("Stun", new object[] { this.gameObject.name, _stunTime });
            }
            else
            {
                dmg = damage;
                blocked = false;
            }
        }
        else if (attackType == "MeleeVertical")
        {
            if (_isBlockingUp)
            {
                dmg = (_perfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("Stun", new object[] { this.gameObject.name, _stunTime });
            }
            else if (_isBlocking)
            {
                dmg = (_imperfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("BlockSound");
                if (_isKnockBackAttack) EventManager.DispatchEvent("DoKnockBack", new object[] { this.gameObject.name });
            }
            else
            {
                dmg = damage;
                blocked = false;
            }
        }
        else if (attackType == "ParryAttack")
        {
            if (GetComponent<PlayerCombat>().isAttacking) EventManager.DispatchEvent("Stun", new object[] { "Me", _stunTime });
            dmg = 10;
            blocked = false;
        }
        else if (attackType == "GuardBreakAttack")
        {
            if (_isBlocking || _isBlockingUp)
            {
                EventManager.DispatchEvent("Stun", new object[] { "Me", _stunTime });
                EventManager.DispatchEvent("GuardBreak", new object[] { "Me", _stunTime * 2 });
            }
            dmg = damage * 1.6f;
            blocked = false;
        }
        else
        {
            dmg = damage;
            blocked = false;
        }

        if (!blocked) EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), attackType });
        else EventManager.DispatchEvent("BlockParticle", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });

        float fill = (Hp - dmg) / maxHp;
        if (fill < 0) fill = 0;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, (dmg > Hp ? 0 : Hp - dmg), fill });
        Hp = dmg >= Hp ? 0 : Hp - dmg;

    }

    void LoseHP(float damage, string attackType, string attackerName)
    {
        float dmg;
        bool blocked = true;

        if (attackType == "MeleeHorizontal")
        {
            if (_isBlockingUp)
            {
                dmg = (_imperfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("BlockSound");
                if (_isKnockBackAttack) EventManager.DispatchEvent("DoKnockBack", new object[] { this.gameObject.name });
            }
            else if (_isBlocking)
            {
                dmg = (_perfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("Stun", new object[] { this.gameObject.name, _stunTime });
            }
            else
            {
                dmg = damage;
                blocked = false;
            }
        }
        else if (attackType == "MeleeVertical")
        {
            if (_isBlockingUp)
            {
                dmg = (_perfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("Stun", new object[] { this.gameObject.name, _stunTime });
            }
            else if (_isBlocking)
            {
                dmg = (_imperfectBlockPerc * damage) / 100;
                EventManager.DispatchEvent("BlockSound");
                if (_isKnockBackAttack) EventManager.DispatchEvent("DoKnockBack", new object[] { this.gameObject.name });
            }
            else
            {
                dmg = damage;
                blocked = false;
            }
        }
        else if (attackType == "ParryAttack")
        {
            if (GetComponent<PlayerCombat>().isAttacking) EventManager.DispatchEvent("Stun", new object[] { attackerName, _stunTime });
            dmg = 10;
            blocked = false;
        }
        else if (attackType == "GuardBreakAttack")
        {
            if (_isBlocking || _isBlockingUp)
            {
                EventManager.DispatchEvent("Stun", new object[] { attackerName, _stunTime });
                EventManager.DispatchEvent("GuardBreak", new object[] { attackerName, _stunTime * 2 });
            }
            dmg = damage * 1.6f;
            blocked = false;
        }
        else
        {
            dmg = damage;
            blocked = false;
        }

        if (!blocked) EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), attackType, uncancelableAttack });
        else EventManager.DispatchEvent("BlockParticle", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });

        float fill = (Hp - dmg) / maxHp;
        if (fill < 0) fill = 0;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, (dmg > Hp ? 0 : Hp - dmg), fill });
        Hp = dmg >= Hp ? 0 : Hp - dmg;

        GetComponent<Player1Input>().GetCamera.ShakeCamera(.1f, .01f);
    }
    void RegainHp(float regained)
    {
        Hp += regained;
        float fill = Hp / maxHp;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, Hp, fill });
    }

    [System.Obsolete("No se usa más, usar la que toma un string como segundo parámetro")]
    public void TakeDamage(float damage)
    {
        EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });
        LoseHP(damage, "");
    }

    [System.Obsolete("Usar la que pide nombre de jugador")]
    public void TakeDamage(float damage, string attackType)
    {
        //EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), attackType });
        LoseHP(damage, attackType);
    }

    public void TakeDamage(float damage, string attackType, string attackerName)
    {
        //EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), attackType });
        LoseHP(damage, attackType, attackerName);
    }

    /// <summary>
    /// Para pasar la normal del polígono y que las partículas aparezcan
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attackType"></param>
    /// <param name="polyNormal"></param>
    [System.Obsolete("Anda para el ojete, no usar/fixear")]
    public void TakeDamage(float damage, string attackType, Vector3 polyNormal)
    {
        EventManager.DispatchEvent("CharacterDamaged", new object[] { this.gameObject.name, polyNormal, this.GetComponent<PlayerParticles>(), attackType });
        LoseHP(damage, "");
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
    private void OnRestartRound(params object[] paramsContainer)
    {
        Hp = maxHp;
        Mana = maxMana;
        var hpFill = Hp / maxHp;
        var manaFill = Mana / maxMana;
        EventManager.DispatchEvent("LifeUpdate", new object[] { this.gameObject.name, Hp, hpFill });
        EventManager.DispatchEvent("ManaUpdate", new object[] { Mana, manaFill, this.gameObject.name });

        var ssCont = GameObject.Find("SpawnSpots");

        transform.position = gameObject.name == "Player1" ? ssCont.transform.Find("SpawnSpotP1").transform.position : ssCont.transform.Find("SpawnSpotP2").transform.position;
        transform.forward = gameObject.name == "Player1" ? ssCont.transform.Find("SpawnSpotP1").transform.forward : ssCont.transform.Find("SpawnSpotP2").transform.forward;


        isDead = false;

        if (!(bool)paramsContainer[0])
        {
            _gameInCourse = true;
            InvokeRepeating("Regenerate", _regenMult, _regenMult);
        }
        else
        {
            EventManager.RemoveEventListener("SpellCasted", OnSpellCasted);
            EventManager.RemoveEventListener("GameFinished", OnGameFinished);
            EventManager.RemoveEventListener("Blocking", OnBlocking);
            EventManager.RemoveEventListener("PlayerDeath", OnPlayerDeath);
            EventManager.RemoveEventListener("CharacterDamaged", OnCharacterDamaged);
            EventManager.RemoveEventListener("KnockBackEnter", OnKnockBackEnter);
            EventManager.RemoveEventListener("KnockBackExit", OnKnockBackExit);
            EventManager.RemoveEventListener("SpecialAttack", OnSpecialAttackUpdate);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);

            this.gameObject.SetActive(false);
        }
    }

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
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            _isBlocking = (bool)paramsContainer[1];
            _isBlockingUp = (bool)paramsContainer[2];
        }
    }

    private void OnPlayerDeath(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            CancelInvoke();
            EventManager.DispatchEvent("IsDead", new object[] { this.gameObject.name, true });
        }

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetDeathOn", PhotonTargets.All);
    }

    #region Cambios Iván 5/10
    //Agregada comprobación para hacer ataques imbloqueables
    /// <summary>
    /// 0 - Sender name
    /// 1 - transform.position
    /// 2 - PlayerParticles
    /// 3 - attackType
    /// 4 - AttackcanBeCanceled
    /// </summary>
    /// <param name="paramsContainer"></param>
    private void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            //isDamaged = true;
            isDamaged = !(bool)paramsContainer[4];
           _isBlocking = false;
            _isBlockingUp = false;
            EventManager.DispatchEvent("IsDamaged", new object[] { this.gameObject.name, isDamaged });

            if (!PhotonNetwork.offlineMode) photonView.RPC("SetDamageOn", PhotonTargets.All);
        }
    }
    #endregion

    private void OnDamageExit()
    {
        isDamaged = false;
        EventManager.DispatchEvent("IsDamaged", new object[] { this.gameObject.name, isDamaged });

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetDamageOff", PhotonTargets.All);
    }

    private void OnKnockBackEnter(params object[] paramsContainer)
    {
        if (this.gameObject.name != (string)paramsContainer[0])
            _isKnockBackAttack = true;
    }

    private void OnKnockBackExit(params object[] paramsContainer)
    {
        if (this.gameObject.name != (string)paramsContainer[0])
            _isKnockBackAttack = false;
    }

    private void OnStunAttackEnter(params object[] paramsContainer)
    {
        if (this.gameObject.name != (string)paramsContainer[0])
            _isStunAttack = true;
    }

    private void OnStunAttackExit(params object[] paramsContainer)
    {
        if (this.gameObject.name != (string)paramsContainer[0])
            _isStunAttack = false;
    }

    void OnSpecialAttackUpdate(object[] paramsContainer)
    {
        var sender = (string)paramsContainer[0];
        var activate = (bool)paramsContainer[1];
        if (sender == gameObject.name)
        {
            uncancelableAttack = activate;
            print("Sender: " + sender + " || UncancelableAttack? " + uncancelableAttack);
        }
    }
    #endregion
}
