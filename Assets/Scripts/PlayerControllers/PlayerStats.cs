using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private bool _gameInCourse;
    private bool _isBlocking = false;
    private bool _isBlockingUp = false;
    private bool _isKnockBackAttack = false;
    private bool _isStunAttack = false;
    private float _regenMult = 0.16f;
    private float _perfectBlockPerc = 10f;
    private float _imperfectBlockPerc = 45f;
    private float _stunTime = 1.2f;
    float _attackSpeed;
    float _amplifiedDamagePercentage = 1;

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
                if (!isDead) EventManager.DispatchEvent(CharacterEvents.PlayerDeath, new object[] { this.gameObject.name });
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
        EventManager.DispatchEvent(CharacterEvents.ManaUpdate, new object[] { Mana, Mana / maxMana, this.gameObject.name });
        //InvokeRepeating("Regenerate", _regenMult, _regenMult);

        var _attackSpeed = GetComponent<Animator>().GetFloat("attackSpeed");
    }

    #region Initialization

    //This is for multiplayer only
    public void Initialize()
    {
        hp = 500;
        maxHp = 500;
        mana = 0;
        maxMana = 250;
        hpRegeneration = 2;
        manaRegeneration = 5;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener(SkillEvents.SpellCasted, OnSpellCasted);
        EventManager.AddEventListener(GameEvents.GameFinished, OnGameFinished);
        EventManager.AddEventListener(AnimationEvents.Blocking, OnBlocking);
        EventManager.AddEventListener(CharacterEvents.PlayerDeath, OnPlayerDeath);
        EventManager.AddEventListener(CharacterEvents.CharacterDamaged, OnCharacterDamaged);
        EventManager.AddEventListener(AnimationEvents.DamageExit, OnDamageExit);
        EventManager.AddEventListener(AnimationEvents.KnockBackEnter, OnKnockBackEnter);
        EventManager.AddEventListener(AnimationEvents.KnockBackExit, OnKnockBackExit);
        EventManager.AddEventListener(AnimationEvents.SpecialAttack, OnSpecialAttackUpdate);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
        EventManager.AddEventListener(SkillEvents.HolyVigorizationCasted, OnHolyVigorizationCasted);
        EventManager.AddEventListener(SkillEvents.HolyVigorizationEnded, OnHolyVigorizationEnded);
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
        EventManager.DispatchEvent(CharacterEvents.ManaUpdate, new object[] { Mana, fill, this.gameObject.name });
    }

    public void RegainMana(float regained)
    {
        Mana += regained;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent(CharacterEvents.ManaUpdate, new object[] { Mana, fill, this.gameObject.name });
    }

    void LoseHP(float damage, string attackType, string attackerName)
    {
        float dmg;
        //bool blocked = true;
        // 0 - Not blocked
        // 1 - Block without particle / special particle
        // 2 - Blocked with particle
        byte blockPhase = 0;

        if (attackType == "MeleeHorizontal")
        {
            if (_isBlockingUp)
            {
                dmg = (_imperfectBlockPerc * damage * _amplifiedDamagePercentage) / 100;
                EventManager.DispatchEvent(SoundEvents.AngelBlock);
                if (_isKnockBackAttack) EventManager.DispatchEvent(CharacterEvents.DoKnockBack, new object[] { this.gameObject.name });
                blockPhase = 2;
            }
            else if (_isBlocking)
            {
                dmg = (_perfectBlockPerc * damage * _amplifiedDamagePercentage) / 100;
                EventManager.DispatchEvent(CharacterEvents.Stun, new object[] { this.gameObject.name, _stunTime });
                blockPhase = 2;
            }
            else
            {
                dmg = damage * _amplifiedDamagePercentage;
                blockPhase = 0;
            }
        }
        else if (attackType == "MeleeVertical")
        {
            if (_isBlockingUp)
            {
                dmg = (_perfectBlockPerc * damage * _amplifiedDamagePercentage) / 100;
                EventManager.DispatchEvent(CharacterEvents.Stun, new object[] { this.gameObject.name, _stunTime });
                blockPhase = 2;
            }
            else if (_isBlocking)
            {
                dmg = (_imperfectBlockPerc * damage * _amplifiedDamagePercentage) / 100;
                EventManager.DispatchEvent(SoundEvents.AngelBlock);
                if (_isKnockBackAttack) EventManager.DispatchEvent(CharacterEvents.DoKnockBack, new object[] { this.gameObject.name });
                blockPhase = 2;
            }
            else
            {
                dmg = damage * _amplifiedDamagePercentage;
                blockPhase = 0;
            }
        }
        else if (attackType == "ParryAttack")
        {
            if (GetComponent<PlayerCombat>().isAttacking) EventManager.DispatchEvent(CharacterEvents.Stun, new object[] { attackerName, _stunTime });
            dmg = 10 * _amplifiedDamagePercentage;
            blockPhase = 0;
        }
        else if (attackType == "GuardBreakAttack")
        {
            if (_isBlocking || _isBlockingUp)
            {
                EventManager.DispatchEvent(CharacterEvents.GuardBreak, new object[] { attackerName, _stunTime * 2 });
                EventManager.DispatchEvent(CharacterEvents.Stun, new object[] { attackerName, _stunTime });
            }
            dmg = damage * 1.6f * _amplifiedDamagePercentage;
            blockPhase = 0;
        }
        else if (attackType == "Spell")
        {
            dmg = damage;
            blockPhase = 0;
        }
        else if (attackType == "Health reduction")
        {
            dmg = damage;
            blockPhase = 1;
        }
        else
        {
            dmg = damage;
            blockPhase = 0;
        }

        if (blockPhase == 0)
        {
            EventManager.DispatchEvent(CharacterEvents.CharacterDamaged, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), attackType, uncancelableAttack });
            EventManager.DispatchEvent(UIEvents.UpdateComboMeter, new object[] { this.gameObject.name });
        }
        else if (blockPhase == 1)
        {
            EventManager.DispatchEvent(ParticleEvents.ToxicDamageParticle, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });
            //EventManager.DispatchEvent(UIEvents.UpdateComboMeter, new object[] { this.gameObject.name });
        }
        else EventManager.DispatchEvent(ParticleEvents.BlockParticle, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>() });

        float fill = (Hp - dmg) / maxHp;
        if (fill < 0) fill = 0;
        EventManager.DispatchEvent(CharacterEvents.LifeUpdate, new object[] { this.gameObject.name, (dmg > Hp ? 0 : Hp - dmg), fill });
        Hp = dmg >= Hp ? 0 : Hp - dmg;

        GetComponent<PlayerInput>().GetCamera.ShakeCamera(1f, .3f);
    }
    void RegainHp(float regained)
    {
        Hp += regained;
        float fill = Hp / maxHp;
        EventManager.DispatchEvent(CharacterEvents.LifeUpdate, new object[] { this.gameObject.name, Hp, fill });
    }

    public void TakeDamage(float damage, string attackType, string attackerName)
    {
        if (gameObject.name != attackerName) LoseHP(damage, attackType, attackerName);
    }

    public void TakeDamage(float damage, string attackType, string attackerName, float time, int tickCoeficient)
    {
        if (gameObject.name != attackerName) StartCoroutine(TakeDamagePerSecond(damage, attackType, attackerName, time, tickCoeficient));
    }

    IEnumerator TakeDamagePerSecond(float damage, string attackType, string attackerName, float time, int tickCoeficient)
    {
        var tickDuration = time / tickCoeficient;
        var ticks = 0;
        float actualDuration = 0;
        var appliedDamage = damage / tickCoeficient;
        while (actualDuration <= time)
        {
            ticks++;

            if (ticks == 30 || ticks == 60)
                LoseHP(appliedDamage, attackType, attackerName);
            else
                LoseHP(appliedDamage, "Health reduction", attackerName);

            actualDuration += tickDuration;
            yield return new WaitForSeconds(tickDuration);
        }
    }

    IEnumerator HolyVigorizationDuration(float duration, float amplifyDamageIncrement, float atkSpeedIncrement)
    {
        var oldIncomingDamagePercentage = _amplifiedDamagePercentage;
        var oldAtkSpeed = _attackSpeed;

        _amplifiedDamagePercentage = amplifyDamageIncrement;
        _attackSpeed = atkSpeedIncrement;

        GetComponent<Animator>().SetBool("berserkOn", true);
        transform.Find("angelBerserk").gameObject.SetActive(true);


        yield return new WaitForSeconds(duration);

        _amplifiedDamagePercentage = oldIncomingDamagePercentage;
        _attackSpeed = oldAtkSpeed;

        GetComponent<Animator>().SetBool("berserkOn", false);

        EventManager.DispatchEvent(SkillEvents.HolyVigorizationEnded, gameObject.name);
        transform.Find("angelBerserk").gameObject.SetActive(false);

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
        Mana = 0;
        var hpFill = Hp / maxHp;
        var manaFill = Mana / maxMana;
        EventManager.DispatchEvent(CharacterEvents.LifeUpdate, new object[] { this.gameObject.name, Hp, hpFill });
        EventManager.DispatchEvent(CharacterEvents.ManaUpdate, new object[] { Mana, manaFill, this.gameObject.name });

        var ssCont = GameObject.Find("SpawnSpots");

        isDead = false;

        transform.position = gameObject.name == "Player1" ? ssCont.transform.Find("SpawnSpotP1").transform.position : ssCont.transform.Find("SpawnSpotP2").transform.position;
        transform.forward = gameObject.name == "Player1" ? ssCont.transform.Find("SpawnSpotP1").transform.forward : ssCont.transform.Find("SpawnSpotP2").transform.forward;

        if (!(bool)paramsContainer[0])
        {
            _gameInCourse = true;
        }
        else
        {
            EventManager.RemoveEventListener(SkillEvents.SpellCasted, OnSpellCasted);
            EventManager.RemoveEventListener(GameEvents.GameFinished, OnGameFinished);
            EventManager.RemoveEventListener(AnimationEvents.Blocking, OnBlocking);
            EventManager.RemoveEventListener(CharacterEvents.PlayerDeath, OnPlayerDeath);
            EventManager.RemoveEventListener(CharacterEvents.CharacterDamaged, OnCharacterDamaged);
            EventManager.RemoveEventListener(AnimationEvents.KnockBackEnter, OnKnockBackEnter);
            EventManager.RemoveEventListener(AnimationEvents.KnockBackExit, OnKnockBackExit);
            EventManager.RemoveEventListener(AnimationEvents.SpecialAttack, OnSpecialAttackUpdate);
            EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);

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
            EventManager.DispatchEvent(CharacterEvents.IsDead, new object[] { this.gameObject.name, true });
        }
    }

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
            EventManager.DispatchEvent(CharacterEvents.IsDamaged, new object[] { this.gameObject.name, isDamaged });
        }
    }

    private void OnDamageExit(params object[] paramasContainer)
    {
        if (this.gameObject.name == (string)paramasContainer[0])
        {
            isDamaged = false;
            EventManager.DispatchEvent(CharacterEvents.IsDamaged, new object[] { this.gameObject.name, isDamaged });
        }
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
        }
    }

    void OnHolyVigorizationEnded(object[] paramsContainer)
    {
        var sender = (string)paramsContainer[0];
        if (sender == gameObject.name)
        {

        }
    }

    void OnHolyVigorizationCasted(object[] paramsContainer)
    {
        var sender = (string)paramsContainer[0];
        if (sender == gameObject.name)
        {
            var duration = (float)paramsContainer[1];
            var amplifyDamageIncrement = (float)paramsContainer[2];
            var attackSpeedIncrement = (float)paramsContainer[3];

            GetComponent<Animator>().SetBool("berserkActivate", false);

            StartCoroutine(HolyVigorizationDuration(duration, amplifyDamageIncrement, attackSpeedIncrement));
        }
    }

    #endregion
}
