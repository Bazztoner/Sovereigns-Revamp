using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEvents
{
    /// <summary>
    /// 0 - Player name (string)
    /// </summary>
    public const string PlayerDeath = "PlayerDeath";

    /// <summary>
    /// 0 - Actual mana (float)
    /// 1 - ManaBar fill (float)
    /// 2 - Sender name (string)
    /// /// </summary>
    public const string ManaUpdate = "ManaUpdate";

    /// <summary>
    /// 0 - Sender name (string)
    /// 1 - Actual hp (float)
    /// 2 - HpBar fill (float)
    /// /// </summary>
    public const string LifeUpdate = "LifeUpdate";

    /// <summary>
    /// 0 - Sender name (string)
    /// </summary>
    public const string DoKnockBack = "DoKnockBack";

    /// <summary>
    /// 0 - Sender name (string)
    /// 1 - Duration (float)
    /// </summary>
    public const string Stun = "Stun";

    /// <summary>
    /// 0 - Sender name (string)
    /// 1 - Duration (float)
    /// </summary>
    public const string GuardBreak = "GuardBreak";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Sender position (Vector3)
    /// 2 - Sender particles (PlayerParticles)
    /// 3 - AttackType (string)
    /// 4 - uncancelableAttack (bool)
    /// </summary>
    public const string CharacterDamaged = "CharacterDamaged";

    /// <summary>
    /// 0 - Sender name (string)
    /// 1 - IsDead? (bool)
    /// </summary>
    public const string IsDead = "IsDead";

    /// <summary>
    /// 0 - Sender name (string)
    /// 1 - IsDamaged? (bool)
    /// </summary>
    public const string IsDamaged = "IsDamaged";

}
