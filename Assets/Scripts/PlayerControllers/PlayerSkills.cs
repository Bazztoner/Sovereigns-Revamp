using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : Photon.MonoBehaviour
{
    #region Variables
    private ISpell _destructiveSkill;
    private ISpell _arcaneOrbSkill;
    private ISpell _repulsiveSkill;
    private ISpell _blinkSkill;
    private Transform _skillPos;
    private float _mana;

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
        _destructiveSkill = new AtractiveTelekinesis();
        _destructiveSkill.Init();

        _arcaneOrbSkill = new SK_ArcaneOrb();
        _arcaneOrbSkill.Init(GetComponent<PlayerMovement>());

        //_repulsiveSkill = new RepulsiveTelekinesis();
        //_repulsiveSkill.Init();

        _blinkSkill = new Blink();
        _blinkSkill.Init(GetComponent<PlayerMovement>());

        _skillPos = transform.Find("SpellPos");

        StartCoroutine(PutAtractiveVisible(_destructiveSkill.GetManaCost()));
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
    /// <summary>Activates the destructive skill</summary>
    public void DestructiveSkill(float mana)
    {
        _mana = mana;
        UseSkill(_destructiveSkill, HUDController.Spells.Environmental, _skillPos);
    }

    /// <summary>Activates the gravitational skill</summary>
    public void GravitationalSkill(float mana)
    {
        _mana = mana;
        UseSkill(_arcaneOrbSkill, HUDController.Spells.Class, _skillPos);
    }

    /// <summary>Activates the repulsive skill</summary>
    public void RepulsiveSkill(float mana)
    {
        _mana = mana;
        UseSkill(_repulsiveSkill, HUDController.Spells.Picked, _skillPos);
    }

    /// <summary>Activates the blink skill</summary>
    public void BlinkSkill(float mana)
    {
        _mana = mana;
        UseSkill(_blinkSkill, HUDController.Spells.Mobility, null);
    }

    /// <summary>Decides if it needs to wait or not to launch an spell</summary>
    private void UseSkill(ISpell spell, HUDController.Spells pickType, Transform skillPos)
    {
        var canCast = !spell.IsInCooldown() && spell.GetManaCost() < _mana;

        if (spell.GetCastType() == CastType.INSTANT)
        {
            if (canCast)
            {
                spell.UseSpell(skillPos);
                EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
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
        else if(spell.GetCastType() == CastType.TWO_STEP)
        {
            if (canCast)
            {
                /*if (_spellsWithPhases.ContainsKey(spell)) _spellsWithPhases[spell] = _spellsWithPhases[spell] > 0 ? 0 : 1;
                else _spellsWithPhases.Add(spell, 0);*/

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
        var vTemp = new Vector3(transform.position.x, transform.position.y + 0.66f, transform.position.z);
        EventManager.DispatchEvent("RepulsiveTelekinesisLoad", new object[] { vTemp, this.GetComponent<PlayerParticles>() });

        isChannelingSpell = true;

        var w = new WaitForSeconds(time);
        yield return w;

        _repulsiveSkill.UseSpell(skillPos);
        isChannelingSpell = false;


        EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
        StartCoroutine(SpellCooldown(spell, pickType));
    }

    /// <summary>Puts the selected skill in cooldown</summary>
    IEnumerator SpellCooldown(ISpell spell, HUDController.Spells pickType)
    {
        spell.EnterInCooldown();
        if (spell.GetType() == typeof(AtractiveTelekinesis))
        {
            ActivateDestructibleProyections(false);
        }

        yield return new WaitForSeconds(spell.CooldownTime());

        spell.ExitFromCooldown();
        if (spell.GetType() == typeof(AtractiveTelekinesis))
        {
            StartCoroutine(PutAtractiveVisible(spell.GetManaCost()));
        }
    }

    IEnumerator PutAtractiveVisible(float cost)
    {
        while (GetActualMana() < cost)
        {
            yield return new WaitForEndOfFrame();
        }

        ActivateDestructibleProyections(true);
    }

    void ActivateDestructibleProyections(bool activate)
    {
        EventManager.DispatchEvent("ChangeStateDestuctibleProjections", new object[] { activate });
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
