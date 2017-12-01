using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    void Init();
    void Init(PlayerMovement character);

    void UseSpell();
    void UseSpell(Transform caster);

    float CooldownTime();

    float CastTime();

    CastType GetCastType();

    bool IsInCooldown();

    void EnterInCooldown();
    void ExitFromCooldown();

    int GetManaCost();

    bool CanBeUsed(float mana);
    bool CanBeUsed(float mana, float distance);
}

/// <summary>
/// Enum de tipos de casteo.
/// INSTANT significa que al momento de pulsar la tecla se ejecuta el spell
/// DELAYED significa que tiene un pequeño tiempo de casteo para ejecutarse
/// CONTINUOUS significa que mientras se mantenga casteando el spell funcionará
/// </summary>
public enum CastType
{
    INSTANT,
    DELAYED,
    CONTINUOUS,
    TWO_STEP
}
