using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : Photon.MonoBehaviour
{
    #region Variables
    private ISpell _environmentalSkill;
    private ISpell _classSkill;
    private ISpell _universalSkill;
    private ISpell _movementSkill;
    private Transform _skillPos;
    public float mana;
    ISpell _actualSkill;
    HUDController.Spells _actualSkillType;

    Dictionary<ISpell, int> _spellsWithPhases = new Dictionary<ISpell, int>();

    [HideInInspector]
    public bool isPulling = false;
    /// <summary>
    /// Canalizado: Funciona mientras es canalizado.
    /// </summary>
    [HideInInspector]
    public bool isChannelingSpell = false;
    /// <summary>
    /// Retrasadado xd: Tarda x tiempo en lanzarse
    /// </summary>
    [HideInInspector]
    public bool isCastingSpell = false;
    /// <summary>
    /// Varias fases: Cada uso del spell cambia de fase al spell. Al ser lanzado en la última fase, el spell se da por terminado
    /// (ej: se crea un escudo en fase 0, en fase 1 se convierte en una energía que daña por cercanía, en fase 2 se lanza el proyectil y ahi se da por terminado)
    /// </summary>
    [HideInInspector]
    public bool isPhasingSpell = false;
    [HideInInspector]
    public bool gtHasObject = false;
    #endregion

    void Start()
    {
        InitializeVariables();
        AddEvents();
    }

    float GetActualMana()
    {
        return GetComponent<PlayerStats>().Mana;
    }

    #region Initialization
    private void InitializeVariables()
    {
        _environmentalSkill = new AtractiveTelekinesis();
        _environmentalSkill.Init();
       

        if (gameObject.name == "Player1" || gameObject.name == "Player3")
        {
            _classSkill = new SK_ArcaneOrb();
            _classSkill.Init(GetComponent<PlayerMovement>());
        }
        else
        {
            _classSkill = new SK_ToxicBlood();
            _classSkill.Init(GetComponent<PlayerMovement>());
        }

        _actualSkill = _classSkill;
        _actualSkillType = HUDController.Spells.Class;

        if (gameObject.name == "Player2")
        {
            _universalSkill = new SK_DoubleEdgedScales();
            _universalSkill.Init(GetComponent<PlayerMovement>());
        }

        _movementSkill = new Blink();
        _movementSkill.Init(GetComponent<PlayerMovement>());

        _skillPos = transform.Find("SpellPos");

        EventManager.DispatchEvent("UISpellChanged", new object[] { _actualSkillType, gameObject.name });

        StartCoroutine(PutAtractiveVisible((AtractiveTelekinesis)_environmentalSkill));
        StartCoroutine(UpdateSkillsState());
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("ObjectPulled", OnObjectPulled);
        EventManager.AddEventListener("ObjectPulling", OnObjectPulling);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
    }
    #endregion

    #region Skills
    public void UseSkill(float mana)
    {
        //this.mana = mana;
        ActivateSkill(_actualSkill, _actualSkillType, _skillPos);
    }

    /// <summary>Activates the destructive skill</summary>
    public void EnvironmentalSkill()
    {
        _actualSkill = _environmentalSkill;
        _actualSkillType = HUDController.Spells.Environmental;
        EventManager.DispatchEvent("UISpellChanged", new object[] { _actualSkillType, gameObject.name });
    }

    /// <summary>Activates the gravitational skill</summary>
    public void ClassSkill()
    {
        _actualSkill = _classSkill;
        _actualSkillType = HUDController.Spells.Class;
        EventManager.DispatchEvent("UISpellChanged", new object[] { _actualSkillType, gameObject.name });
    }

    /// <summary>Activates the repulsive skill</summary>
    public void UniversalSkill()
    {
        _actualSkill = _universalSkill;
        _actualSkillType = HUDController.Spells.Universal;
        EventManager.DispatchEvent("UISpellChanged", new object[] { _actualSkillType, gameObject.name });
    }

    /// <summary>Activates the blink skill</summary>
    public void MovementSkill(float mana)
    {
        //this.mana = mana;
        ActivateSkill(_movementSkill, HUDController.Spells.Mobility, null);
    }

    IEnumerator CastToxicBlood(float time, ISpell spell, HUDController.Spells pickType, Transform skillPos)
    {
        EventManager.DispatchEvent("ToxicBloodCasted", new object[] { gameObject.name });
        var w = new WaitForSeconds(time);
        yield return w;
        spell.UseSpell(skillPos);
        EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
        EventManager.DispatchEvent("ToxicBloodStopCasted", new object[] { gameObject.name });
    }

    /// <summary>Decides if it needs to wait or not to launch an spell</summary>
    private void ActivateSkill(ISpell spell, HUDController.Spells pickType, Transform skillPos)
    {
        var canCast = !spell.IsInCooldown() && spell.GetManaCost() < mana;

        if (spell.GetCastType() == CastType.INSTANT)
        {
            if (canCast)
            {
                StartCoroutine(CastToxicBlood(0.25F, spell, pickType, skillPos));
                StartCoroutine(SpellCooldown(spell, pickType));
            }
        }
        else if (spell.GetCastType() == CastType.DELAYED)
        {
            if (canCast)
            {
                StartCoroutine(CastSpell(spell.CastTime(), spell, pickType, skillPos));
            }
        }
        else if (spell.GetCastType() == CastType.TWO_STEP)
        {
            if (canCast)
            {
                if (!_spellsWithPhases.ContainsKey(spell)) _spellsWithPhases.Add(spell, 0);

                if (_spellsWithPhases[spell] == 0)
                {
                    isPhasingSpell = true;
                    _spellsWithPhases[spell]++;
                }
                else
                {
                    isPhasingSpell = false;
                    _spellsWithPhases[spell] = 0;
                }
                spell.UseSpell(skillPos);
                EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
                StartCoroutine(SpellCooldown(spell, pickType));
            }
        }
    }
    #endregion

    #region Coroutines
    /// <summary>Waits a certain time to launch the spell</summary>
    IEnumerator CastSpell(float time, ISpell spell, HUDController.Spells pickType, Transform skillPos)
    {
        EventManager.DispatchEvent("DoubleEdgedScaleCasted", new object[] { gameObject.name });
        var vTemp = new Vector3(transform.position.x, transform.position.y + 0.66f, transform.position.z);
        EventManager.DispatchEvent("SpellBeingCasted", new object[] { vTemp, this.GetComponent<PlayerParticles>() });

        isChannelingSpell = true;

        var w = new WaitForSeconds(time);
        yield return w;

        _universalSkill.UseSpell(skillPos);
        isChannelingSpell = false;

        EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
        EventManager.DispatchEvent("DoubleEdgedScaleStopCasted", new object[] { gameObject.name });
        StartCoroutine(SpellCooldown(spell, pickType));
    }

    /// <summary>Puts the selected skill in cooldown</summary>
    IEnumerator SpellCooldown(ISpell spell, HUDController.Spells pickType)
    {
        spell.EnterInCooldown();

        yield return new WaitForSeconds(spell.CooldownTime());

        spell.ExitFromCooldown();
    }

    IEnumerator UpdateSkillsState()
    {
        var skills = new ISpell[4] { _environmentalSkill, _classSkill, _universalSkill, _movementSkill };
        while ("Santiago Maldonado" != "Lo mató Gendarmería")
        {
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    var canBeUsed = skills[i].CanBeUsed(mana, GetComponent<PlayerMovement>().DistanceToEnemy(GetComponent<PlayerInput>().GetCamera.transform));
                    EventManager.DispatchEvent("UIUpdateSkillState", new object[] { canBeUsed, i, gameObject.name });
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PutAtractiveVisible(AtractiveTelekinesis spell)
    {
        while (true)
        {
            if (_actualSkillType == HUDController.Spells.Environmental)
                ActivateDestructibleProyections(spell.CanBeUsed(GetActualMana()));
            else ActivateDestructibleProyections(false);
            yield return new WaitForEndOfFrame();
        }
    }

    void ActivateDestructibleProyections(bool activate)
    {
        EventManager.DispatchEvent("ChangeStateDestuctibleProjections", new object[] { activate, gameObject.name });
    }
    #endregion

    #region Events
    void OnObjectPulling(params object[] paramsContainer)
    {
        isPulling = true;
    }

    void OnObjectPulled(params object[] paramsContainer)
    {
        isPulling = false;
    }

    void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
            isChannelingSpell = false;
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener("ObjectPulled", OnObjectPulled);
            EventManager.RemoveEventListener("ObjectPulling", OnObjectPulling);
            EventManager.RemoveEventListener("CharacterDamaged", OnCharacterDamaged);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);
        }
    }
    #endregion

    #region Telekinetic Physics
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Utilities.IntLayers.TELEKINESISOBJECT)
            collision.gameObject.GetComponent<TelekineticObject>().ChangeState(PhotonNetwork.player.NickName);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == Utilities.IntLayers.TELEKINESISOBJECT)
            collision.gameObject.GetComponent<TelekineticObject>().ChangeState(PhotonNetwork.player.NickName);
    }

    #endregion
}
