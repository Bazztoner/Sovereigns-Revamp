using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Photon.MonoBehaviour
{

    #region Variables
    public int lightAttackDamage = 6;
    public int heavyAttackDamage = 16;

    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isBlocking = false;
    [HideInInspector]
    public bool isBlockingUp = false;
    #endregion

    void Start()
    {
        EventManager.AddEventListener(AnimationEvents.IdleEnter, OnIdleEnter);
        EventManager.AddEventListener(CharacterEvents.CharacterDamaged, OnCharacterDamaged);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener(AnimationEvents.IdleEnter, OnIdleEnter);
            EventManager.RemoveEventListener(CharacterEvents.CharacterDamaged, OnCharacterDamaged);
            EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);
        }
    }

    #region Actions
    /// <summary>Makes a light attack</summary>
    public void DoLightAttack()
    {
        isAttacking = true;

        EventManager.DispatchEvent(AnimationEvents.X, new object[] { this.gameObject.name, true });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetX", PhotonTargets.All);
    }

    /// <summary>Makes a heavy attack</summary>
    public void DoHeavyAttack()
    {
        isAttacking = true;

        EventManager.DispatchEvent(AnimationEvents.Y, new object[] { this.gameObject.name, true });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetY", PhotonTargets.All);
    }

    /// <summary>Informs that an attack is being made to cancel other animations</summary>
    private void SetAttack()
    {
        isBlocking = false;
        EventManager.DispatchEvent(AnimationEvents.Blocking, new object[] { this.gameObject.name, isBlocking, isBlockingUp });
        EventManager.DispatchEvent(ParticleEvents.ActivateRunParticle, new object[] { this.gameObject.name, false });
    }

    /// <summary>Makes the character block up or mid</summary>
    public void Block(bool blockUp)
    {
        isBlocking = true;
        isBlockingUp = blockUp;
        EventManager.DispatchEvent(AnimationEvents.Blocking, new object[] { this.gameObject.name, isBlocking, blockUp });
    }

    /// <summary>Makes the character stop blocking</summary>
    public void StopBlock()
    {
        isBlocking = false;
        EventManager.DispatchEvent(AnimationEvents.Blocking, new object[] { this.gameObject.name, isBlocking, false });
    }
    #endregion

    #region Animation Events
    private void OnIdleEnter(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            isAttacking = false;
            EventManager.DispatchEvent(AnimationEvents.AttackExit);
        }
    }

    //HATES TEAR
    private void OnAttackEnter(string param)
    {
        ///Recibe el evento del AnimEvent. Esto selecciona qué collider va a utilizar.
        EventManager.DispatchEvent(param, gameObject.name);
    }

    //Resive el animation event de los ataques, y trae por parametro el daño que hace, el cual esta configurado desde las animaciones mismas.
    private void OnAttackEnter(int dmg)
    {
        EventManager.DispatchEvent(AnimationEvents.AttackEnter, new object[] { this.gameObject.name, dmg });
    }


    private void OnAttackExit()
    {
        EventManager.DispatchEvent(AnimationEvents.AttackExit);
    }

    private void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            var cancelAttack = (bool)paramsContainer[4];
            isAttacking = cancelAttack;
            isBlocking = false;
            isBlockingUp = false;
        }
    }
    #endregion
}
