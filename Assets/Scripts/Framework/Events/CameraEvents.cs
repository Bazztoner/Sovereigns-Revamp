using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvents
{
    /// <summary>
    /// 0 - Sender (string)
    /// 1 - CameraTransform (Transform)
    /// </summary>
    public const string StopStunCamera = "StopStunCamera";

    /// <summary>
    /// 0 - CameraTransform (Transform)
    /// 1 - Duration (float)
    /// 2 - _canBlock (bool)
    /// </summary>
    public const string StunShake = "StunShake";

    /// <summary>
    /// 0 - Sender (string)
    /// 1 - LockOn? (bool)
    /// 2 - LockTransform (Transform)
    /// 3 - CameraSender (Camera)
    /// </summary>
    public const string LockOnActivated = "LockOnActivated";

    /// <summary>
    /// 0 - CameraSender (Camera)
    /// </summary>
    public const string StartBlinkFeedback = "StartBlinkFeedback";
}
