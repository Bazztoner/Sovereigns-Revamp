using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents
{
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - AttackDamage (int)
    /// </summary>
    public const string AttackEnter = "AttackEnter";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string AttackExit = "AttackExit";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string IdleEnter = "IdleEnter";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string RollExit = "RollExit";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string StopStun = "StopStun";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string DamageExit = "DamageExit";
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Activate (bool)
    /// </summary>
    public const string X = "X";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Activate (bool)
    /// </summary>
    public const string Y = "Y";
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Activate (bool)
    /// </summary>
    public const string SpecialAttack = "SpecialAttack";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string ParryAttack = "ParryAttack";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string GuardBreakAttack = "GuardBreakAttack";
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Activate (bool)
    /// </summary>
    public const string RollingAnimation = "RollingAnimation";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string HorizontalAttack = "HorizontalAttack";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string VerticalAttack = "VerticalAttack";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string KnockBackEnter = "KnockBackEnter";
    /// <summary>
    /// 0 - Sender (string)
    /// </summary>
    public const string KnockBackExit = "KnockBackExit";
    /// <summary>
    /// 0 - Sender (string) 
    /// 1 - xMovement (float)
    /// 2 - yMovement (float)
    /// </summary>
    public const string RunningAnimations = "RunningAnimations";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - Is Blocking? (bool)
    /// 2 - Is Blocking up? (bool)
    /// </summary>
    public const string Blocking = "Blocking";
}
