using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Photon.MonoBehaviour {

    #region Variables
    public int lightAttackDamage = 6;
    public int heavyAttackDamage = 16;
    
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isBlocking = false;
    #endregion

    void Start()
    {
        EventManager.AddEventListener("IdleEnter", OnIdleEnter);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
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
        EventManager.DispatchEvent("Blocking", new object[] { this.gameObject.name, isBlocking });
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
    private void OnIdleEnter(params object[] paramsContainer)
    {
        isAttacking = false;
        //EventManager.DispatchEvent("SetTrailState", new object[] { gameObject.name, false });
    }

    private void OnAttackExit()
    {
        EventManager.DispatchEvent("AttackExit");
        //EventManager.DispatchEvent("SetTrailState", new object[] { gameObject.name, false });
    }

    private void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
        {
            isAttacking = false;
            isBlocking = false;
        }
    }
    #endregion
}
