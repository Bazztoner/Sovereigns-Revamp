using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEvents
{
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string ToxicBloodCasted = "ToxicBloodCasted";

    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string StartCastDoubleEdgedScales = "StartCastDoubleEdgedScales";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Unable to block time (float)
    /// </summary>
    public const string DoubleEdgedScalesCasted = "DoubleEdgedScalesCasted";

    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string ToxicBloodStopCasted = "ToxicBloodStopCasted";

    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string DoubleEdgedScaleStopCasted = "DoubleEdgedScaleStopCasted";

    /// <summary>
    /// 0 - Activate? (bool)
    /// 1 - Sender (string)
    /// </summary>
    public const string ChangeStateDestuctibleProjections = "ChangeStateDestuctibleProjections";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Duration (float)
    /// 2 - selfDamageIncrease (float - percentual)
    /// 3 - attackSpeedIncrease (float - percentual)
    /// </summary>
    public const string HolyVigorizationCasted = "HolyVigorizationCasted";

    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string HolyVigorizationEnded = "HolyVigorizationEnded";

    /// <summary>
    /// 0 - Cooldown (Float)
    /// 1 - SkillType (HUDController.Spells - enum)
    /// 2 - Sender (string)
    /// </summary>
    public const string SpellCooldown = "SpellCooldown";

    /// <summary>
    /// 0 - Position (Vector3)
    /// 1 - SenderParticles (PlayerParticles)
    /// </summary>
    public const string SpellBeingCasted = "SpellBeingCasted";

    /// <summary>
    /// 0 - Mana cost (float)
    /// 1 - Sender (string)
    /// </summary>
    public const string SpellCasted = "SpellCasted";

    /// <summary>
    /// 0 - Position (Vector3)
    /// 1 - SenderParticles (PlayerParticles)
    /// </summary>
    public const string ApplyShockwave = "RepulsiveTelekinesisCasted";

    /// <summary>
    /// 0 - Dummy (DMM_ArcaneOrb)
    /// </summary>
    public const string ArcaneDummyDestroyedByLifeTime = "ArcaneDummyDestroyedByLifeTime";

    /// <summary>
    /// 0 - Projectile (ArcaneOrb)
    /// </summary>
    public const string ArcaneOrbDestroyedByLifeTime = "ArcaneOrbDestroyedByLifeTime";
}
