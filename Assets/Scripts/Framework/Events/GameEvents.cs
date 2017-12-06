using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public const string GameStarted = "GameStarted";

    public const string BeginGame = "BeginGame";

    /// <summary>
    /// 0 - Winner Name (string)
    /// </summary>
    public const string GameFinished = "GameFinished";

    /// <summary>
    /// 0 - Player 1 Score (int)
    /// 1 - Player 2 Score (int)
    /// </summary>
    public const string EndOfMatch = "EndOfMatch";

    public const string EndMatch = "EndMatch";

    /// <summary>
    /// 0 - Bool
    /// </summary>
    public const string RestartRound = "RestartRound";

    /// <summary>
    /// 0 - Bool
    /// </summary>
    public const string DoConnect = "DoConnect";
    /// <summary>
    /// 0 - Bool
    /// </summary>
    public const string DoNotConnect = "DoNotConnect";
    /// <summary>
    /// 0 - Bool
    /// </summary>
    public const string DoDummyTest = "DoNotConnect";
    /// <summary>
    /// 0 - Bool
    /// </summary>
    public const string DividedScreen = "DoNotConnect";
}
