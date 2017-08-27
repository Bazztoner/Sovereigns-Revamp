using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Photon.MonoBehaviour {

    #region Variables
    public int lightAttackDamage = 6;
    public int heavyAttackDamage = 16;

    [HideInInspector]
    public bool isAttackAvailable = true;
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isBlocking = false;
    [HideInInspector]
    public bool isLightAttack = false;
    [HideInInspector]
    public bool isHeavyAttack = false;
    [HideInInspector]
    public bool x = false;
    [HideInInspector]
    public bool y = false;
    #endregion

    #region Actions
    /// <summary>Makes a light attack</summary>
    public void DoLightAttack()
    {
        isAttackAvailable = false;
        isAttacking = true;
        isLightAttack = true;
        x = true;

        EventManager.DispatchEvent("AttackEnter", new object[] { this.gameObject.name, lightAttackDamage });
        EventManager.DispatchEvent("X", new object[] { this.gameObject.name, x });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetX", PhotonTargets.All);
    }

    /// <summary>Makes a heavy attack</summary>
    public void DoHeavyAttack()
    {
        isAttackAvailable = false;
        isAttacking = true;
        isHeavyAttack = true;
        y = true;

        EventManager.DispatchEvent("AttackEnter", new object[] { this.gameObject.name, heavyAttackDamage });
        EventManager.DispatchEvent("Y", new object[] { this.gameObject.name, y });

        SetAttack();

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetY", PhotonTargets.All);
    }

    /// <summary>Informs that an attack is being made to cancel other animations</summary>
    private void SetAttack()
    {
        EventManager.DispatchEvent("Attacking");
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking });
        isBlocking = false;
    }

    /// <summary>Makes the character block</summary>
    public void Block()
    {
        isBlocking = true;
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking });
    }

    /// <summary>Makes the character stop blocking</summary>
    public void StopBlock()
    {
        isBlocking = false;
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking });
    }
    #endregion

    #region Animation Events
    private void OnIddleEnter()
    {
        x = false;
        y = false;
        isAttackAvailable = true;
        isLightAttack = false;
        isHeavyAttack = false;
        isAttacking = false;

        EventManager.DispatchEvent("X", new object[] { this.gameObject.name, x });
        EventManager.DispatchEvent("Y", new object[] { this.gameObject.name, y });

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetBothOff", PhotonTargets.All);
    }

    private void OnSyncTime()
    {
        x = false;
        y = false;
        isAttackAvailable = true;

        EventManager.DispatchEvent("X", new object[] { this.gameObject.name, x });
        EventManager.DispatchEvent("Y", new object[] { this.gameObject.name, y });

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetBothOff", PhotonTargets.All);
    }
    #endregion
}
