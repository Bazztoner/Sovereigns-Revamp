using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEvents
{
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Position (Vector3)
    /// 2 - PlayerParticles
    /// 3 - Duration (float)
    /// </summary>
    public const string GuardBreakParticle = "GuardBreakParticle";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Position (Vector3)
    /// 2 - PlayerParticles
    /// 3 - Duration (float)
    /// </summary>
    public const string StunParticle = "StunParticle";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Activate (bool)
    /// </summary>
    public const string ActivateRunParticle = "ActivateRunParticle";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Sender position (Vector3)
    /// 2 - Sender particles (PlayerParticles)
    /// </summary>
    public const string ToxicDamageParticle = "ToxicDamageParticle";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Sender position (Vector3)
    /// 2 - Sender particles (PlayerParticles)
    /// </summary>
    public const string BlockParticle = "BlockParticle";
}
