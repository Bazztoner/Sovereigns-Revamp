/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Variables
    public static InputManager instance;

    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode lightAttack = KeyCode.Mouse0;
    public KeyCode heavyAttack = KeyCode.Mouse1;
    public KeyCode blocking = KeyCode.Space;
    public KeyCode dodge_run = KeyCode.LeftShift;
    public KeyCode movementSkill = KeyCode.F;
    public KeyCode classSkill = KeyCode.E;
    public KeyCode enviromentalSkill = KeyCode.R;
    public KeyCode universalSkill = KeyCode.Q;
    public KeyCode lockOn = KeyCode.Mouse2;
    public KeyCode enviromentAction = KeyCode.LeftControl;
    public KeyCode item1 = KeyCode.Alpha1;
    public KeyCode item2 = KeyCode.Alpha2;
    public KeyCode item3 = KeyCode.Alpha3;
    public KeyCode item4 = KeyCode.Alpha4;

    public KeyCode lightAttackGamepad = KeyCode.JoystickButton2;
    public KeyCode heavyAttackGamepad = KeyCode.JoystickButton3;
    public KeyCode dodge_runGamepad = KeyCode.JoystickButton0;
    public KeyCode useObjectGamepad = KeyCode.JoystickButton1;
    public KeyCode classSkillGamepad = KeyCode.JoystickButton4;
    public KeyCode enviromentSkillGamepad = KeyCode.JoystickButton10;
    public KeyCode skillGamepad = KeyCode.JoystickButton5;
    public KeyCode lockOnGamepad = KeyCode.JoystickButton9;
    public KeyCode enviromentActionGamepad = KeyCode.JoystickButton8;
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
        else if (Input.GetKey(backward)) return -1f;
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
        return (Input.GetKeyDown(lightAttack));
    }

    public bool GetHeavyAttack()
    {
        return (Input.GetKeyDown(heavyAttack));
    }

    public bool GetBlocking()
    {
        return (Input.GetKey(blocking));
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

    public bool GetEnviromentAction()
    {
        return (Input.GetKeyDown(enviromentAction));
    }

    public bool GetItem1()
    {
        return (Input.GetKeyDown(item1));
    }

    public bool GetItem2()
    {
        return (Input.GetKeyDown(item2));
    }

    public bool GetItem3()
    {
        return (Input.GetKeyDown(item3));
    }

    public bool GetItem4()
    {
        return (Input.GetKeyDown(item4));
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
        return (Input.GetKeyDown(lightAttackGamepad));
    }

    public bool GetJoystickHeavyAttack()
    {
        return (Input.GetKeyDown(heavyAttackGamepad));
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
        return (Input.GetKeyDown(useObjectGamepad));
    }

    public bool GetJoystickClassSkill()
    {
        return (Input.GetKeyDown(classSkillGamepad));
    }

    public bool GetJoystickEnviromentSkill()
    {
        return (Input.GetAxis("RT/LT") < 0);
    }

    public bool GetJoystickUniversalSkill()
    {
        return (Input.GetKeyDown(skillGamepad));
    }

    public bool GetJoystickLockOn()
    {
        return (Input.GetKeyDown(lockOnGamepad));
    }

    public bool GetJoystickEnviromentAction()
    {
        return (Input.GetKeyDown(enviromentActionGamepad));
    }

    public bool GetJoystickItem1()
    {
        return (Input.GetAxis("DpadY") > 0);
    }

    public bool GetJoystickItem2()
    {
        return (Input.GetAxis("DpadX") > 0);
    }

    public bool GetJoystickItem3()
    {
        return (Input.GetAxis("DpadY") < 0);
    }

    public bool GetJoystickItem4()
    {
        return (Input.GetAxis("DpadX") < 0);
    }

    #endregion

}
*/