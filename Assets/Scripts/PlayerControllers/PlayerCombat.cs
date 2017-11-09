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
        EventManager.AddEventListener("IdleEnter", OnIdleEnter);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener("IdleEnter", OnIdleEnter);
            EventManager.RemoveEventListener("CharacterDamaged", OnCharacterDamaged);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);
        }
    }

    #region Actions
    /// <summary>Makes a light attack</summary>
    public void DoLightAttack()
    {
        isAttacking = true;

        EventManager.DispatchEvent("X", new object[] { this.gameObject.name, true });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetX", PhotonTargets.All);
    }

    /// <summary>Makes a heavy attack</summary>
    public void DoHeavyAttack()
    {
        isAttacking = true;

        EventManager.DispatchEvent("Y", new object[] { this.gameObject.name, true });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetY", PhotonTargets.All);
    }

    /// <summary>Informs that an attack is being made to cancel other animations</summary>
    private void SetAttack()
    {
        isBlocking = false;
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking, isBlockingUp });
        EventManager.DispatchEvent("ActivateRunParticle", new object[] { this.gameObject.name, false });
    }

    /// <summary>Makes the character block up or mid</summary>
    public void Block(bool blockUp)
    {
        isBlocking = true;
        isBlockingUp = blockUp;
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking, blockUp });
    }

    /// <summary>Makes the character stop blocking</summary>
    public void StopBlock()
    {
        isBlocking = false;
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking, false });
    }
    #endregion

    #region Animation Events
    private void OnIdleEnter(params object[] paramsContainer)
    {
        isAttacking = false;
        EventManager.DispatchEvent("AttackExit");
    }

    private void OnAttackExit()
    {
        EventManager.DispatchEvent("AttackExit");
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
