using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : Photon.MonoBehaviour
{
    #region Variables
    private ISpell _destructiveSkill;
    private ISpell _gravitationalSkill;
    private ISpell _repulsiveSkill;
    private ISpell _blinkSkill;
    private Transform _skillPos;
    private float _mana;

    [HideInInspector]
    public bool isPulling = false;
    [HideInInspector]
    public bool isCastingSpell = false;
    [HideInInspector]
    public bool gtHasObject = false; //Usar esta variable en vez de la que estaba en CharacterMovent
    #endregion

    void Start()
    {
        InitializeVariables();
        AddEvents();
    }

    #region Initialization
    private void InitializeVariables()
    {
        _destructiveSkill = new AtractiveTelekinesis();
        _destructiveSkill.Init();

        _gravitationalSkill = new GravitationalTelekinesis();
        _gravitationalSkill.Init();

        _repulsiveSkill = new RepulsiveTelekinesis1();
        _repulsiveSkill.Init();

        _blinkSkill = new Blink();
        _blinkSkill.Init(GetComponent<PlayerMovement>());

        _skillPos = transform.Find("SpellPos");
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("ObjectPulled", OnObjectPulled);
        EventManager.AddEventListener("ObjectPulling", OnObjectPulling);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
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
        UseSkill(_gravitationalSkill, HUDController.Spells.Class, _skillPos);
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
                //EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType });
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
    }
    #endregion

    #region Coroutines
    /// <summary>Waits a certain time to launch the spell</summary>
    IEnumerator CastSpell(float time, ISpell spell, HUDController.Spells pickType, Transform skillPos)
    {
        var vTemp = new Vector3(transform.position.x, transform.position.y + 0.66f, transform.position.z);
        EventManager.DispatchEvent("RepulsiveTelekinesisLoad", new object[] { vTemp, this.GetComponent<PlayerParticles>() });

        isCastingSpell = true;

        var w = new WaitForSeconds(time);
        yield return w;

        _repulsiveSkill.UseSpell(skillPos);
        isCastingSpell = false;


        EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType, this.gameObject.name });
        StartCoroutine(SpellCooldown(spell, pickType));
    }

    /// <summary>Puts the selected skill in cooldown</summary>
    IEnumerator SpellCooldown(ISpell spell, HUDController.Spells pickType)
    {
        spell.EnterInCooldown();
        if (spell.GetType() == typeof(AtractiveTelekinesis))
        {
            EventManager.DispatchEvent("ChangeStateDestuctibleProjections", new object[] { false });
        }
        yield return new WaitForSeconds(spell.CooldownTime());
        spell.ExitFromCooldown();
        if (spell.GetType() == typeof(AtractiveTelekinesis))
        {
            EventManager.DispatchEvent("ChangeStateDestuctibleProjections", new object[] { true });
        }
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
        if(this.gameObject.name == (string)paramsContainer[0])
            isCastingSpell = false;
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
