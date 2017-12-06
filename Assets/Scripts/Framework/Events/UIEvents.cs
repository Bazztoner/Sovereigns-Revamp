using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
    /// <summary>
    /// 0 - canBeUsed (bool)
    /// 1 - spell index (int) (enum index)
    /// 2 - sender (string)
    /// </summary>
    public const string UpdateSkillState = "UIUpdateSkillState";

    /// <summary>
    /// 0 - actualSkillType (HUDController.Spells - enum)
    /// 1 - sender (string)
    /// </summary>
    public const string SpellChanged = "UISpellChanged";

    /// <summary>
    /// 0 - sender (string)
    /// </summary>
    public const string UpdateComboMeter = "UpdateComboMeter";

    /// <summary>
    /// 0 - Round name (int)
    /// </summary>
    public const string SetRoundText = "SetRoundText";
}

