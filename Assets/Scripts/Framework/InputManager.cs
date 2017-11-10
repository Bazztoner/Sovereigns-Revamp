using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Variables
    public static InputManager instance;

    public KeyCode forward = KeyCode.W;
    public KeyCode backwards = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode horizontalAttack = KeyCode.Mouse0;
    public KeyCode verticalAttack = KeyCode.Mouse1;
    public KeyCode block = KeyCode.Space;
    public KeyCode dodge_run = KeyCode.LeftShift;
    public KeyCode movementSkill = KeyCode.F;
    public KeyCode universalSkill = KeyCode.Alpha1;
    public KeyCode classSkill = KeyCode.Alpha2;
    public KeyCode enviromentalSkill = KeyCode.Alpha3;
    public KeyCode lockOn = KeyCode.Mouse2;
    public KeyCode useSkill = KeyCode.Q;

    public KeyCode horizontalAttackGamepad = KeyCode.JoystickButton2;
    public KeyCode verticalAttackGamepad = KeyCode.JoystickButton3;
    public KeyCode dodge_runGamepad = KeyCode.JoystickButton0;
    public KeyCode movementSkillGamepad = KeyCode.JoystickButton10;

    public KeyCode useSkillGamepad = KeyCode.JoystickButton1;
    public KeyCode lockOnGamepad = KeyCode.JoystickButton9;
    #endregion

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    #region Keyboard
    public float GetVerticalMovement()
    {
        if (Input.GetKey(forward)) return 1f;
        else if (Input.GetKey(backwards)) return -1f;
        else return 0f;
    }

    public float GetHorizontalMovement()
    {
        if (Input.GetKey(left)) return -1f;
        else if (Input.GetKey(right)) return 1f;
        else return 0f;
    }

    public float GetHorizontalCamera()
    {
        if (Input.GetAxis("Horizontal") != 0) return Input.GetAxis("Horizontal");
        else return 0f;
    }

    public float GetVerticalCamera()
    {
        if (Input.GetAxis("Vertical") != 0) return Input.GetAxis("Vertical");
        else return 0f;
    }

    public bool GetLightAttack()
    {
        return (Input.GetKeyDown(horizontalAttack));
    }

    public bool GetHeavyAttack()
    {
        return (Input.GetKeyDown(verticalAttack));
    }

    public bool GetBlocking()
    {
        return (Input.GetKey(block));
    }

    public bool GetDodge()
    {
        return (Input.GetKeyDown(dodge_run));
    }

    public bool GetRun()
    {
        return (Input.GetKey(dodge_run));
    }

    public bool GetMovementSkill()
    {
        return (Input.GetKeyDown(movementSkill));
    }

    public bool GetClassSkill()
    {
        return (Input.GetKeyDown(classSkill));
    }

    public bool GetEnviromentSkill()
    {
        return (Input.GetKeyDown(enviromentalSkill));
    }

    public bool GetUniversalSkill()
    {
        return (Input.GetKeyDown(universalSkill));
    }

    public bool GetLockOn()
    {
        return (Input.GetKeyDown(lockOn));
    }

    public bool GetUseSkill()
    {
        return (Input.GetKeyDown(useSkill));
    }
    #endregion

    #region Joystick

    public float GetJoystickVerticalMovement()
    {
        if (Input.GetAxis("VerticalGamepad") != 0)
            return Input.GetAxis("VerticalGamepad");
        else return 0f;
    }

    public float GetJoystickHorizontalMovement()
    {
        if (Input.GetAxis("HorizontalGamepad") != 0)
            return Input.GetAxis("HorizontalGamepad");
        else return 0f;
    }

    public float GetJoystickHorizontalCamera()
    {
        if (Input.GetAxis("CameraHorizontalGamepad") != 0)
            return Input.GetAxis("CameraHorizontalGamepad");
        else return 0f;
    }

    public float GetJoystickVerticalCamera()
    {
        if (Input.GetAxis("CameraVerticalGamepad") != 0)
            return Input.GetAxis("CameraVerticalGamepad");
        else return 0f;
    }

    public bool GetJoystickLightAttack()
    {
        return (Input.GetKeyDown(horizontalAttackGamepad));
    }

    public bool GetJoystickHeavyAttack()
    {
        return (Input.GetKeyDown(verticalAttackGamepad));
    }

    public bool GetJoystickBlocking()
    {
        return (Input.GetAxis("RT/LT") > 0);
    }

    public bool GetJoystickDodge()
    {
        return (Input.GetKeyDown(dodge_runGamepad));
    }

    public bool GetJoystickRun()
    {
        return (Input.GetKey(dodge_runGamepad));
    }

    public bool GetJoystickMovementSkill()
    {
        return (Input.GetAxis("RT/LT") < 0);
    }

    public bool GetJoystickClassSkill()
    {
        return (Input.GetAxis("DpadY") > 0);
    }

    public bool GetJoystickEnviromentSkill()
    {
        return (Input.GetAxis("DpadX") > 0);
    }

    public bool GetJoystickUniversalSkill()
    {
        return (Input.GetAxis("DpadX") < 0);
    }

    public bool GetJoystickLockOn()
    {
        return (Input.GetKeyDown(lockOnGamepad));
    }

    public bool GetJoystickUseSkill()
    {
        return (Input.GetKeyDown(useSkillGamepad));
    }
    #endregion

}
