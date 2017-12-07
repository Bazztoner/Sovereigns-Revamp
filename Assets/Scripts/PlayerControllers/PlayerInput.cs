using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerSkills))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerParticles))]
[RequireComponent(typeof(PlayerBlinkSpots))]

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _pm;
    private PlayerCombat _pc;
    private PlayerSkills _ps;
    private PlayerStats _pst;
    private Transform _cam;
    private Vector3 _direction;
    private bool _checkingRoll = false;
    private bool _gameInCourse = true;
    private bool _canMove = true;
    private bool _canAttack = true;
    bool _canBlock = true;
    private bool _isStun = false;
    private float _dodgeTime = 0.15f;

    public bool readJoystick;

    public CamRotationController GetCamera
    {
        get
        {
            if (_cam == null)
                FindCamera();
            return _cam.GetComponent<CamRotationController>();
        }
    }

    public Vector3 RunDirection
    {
        get { return _direction; }
    }
    void Start()
    {
        LockCursor();
        Initialize();
        EventManager.AddEventListener(GameEvents.GameFinished, OnGameFinished);
        EventManager.AddEventListener(CharacterEvents.Stun, OnStun);
        EventManager.AddEventListener(AnimationEvents.StopStun, OnStopStun);
        EventManager.AddEventListener(CharacterEvents.GuardBreak, OnGuardBreak);
        EventManager.AddEventListener(SkillEvents.DoubleEdgedScalesCasted, OnDoubleEdgedScalesCasted);
        EventManager.AddEventListener(AnimationEvents.SpecialAttack, OnSpecialAttack);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
    }

    void Update()
    {
        if (_gameInCourse)
        {
            CheckConditions();
            CheckRollAndRun();
            CheckAttacks();
            CheckSkills();
            _ps.mana = _pst.Mana;
        }
    }

    void FixedUpdate()
    {
        if (_gameInCourse) CheckMove();
    }

    #region Initialization
    private void Initialize()
    {
        _pm = GetComponent<PlayerMovement>();
        _pc = GetComponent<PlayerCombat>();
        _ps = GetComponent<PlayerSkills>();
        _pst = GetComponent<PlayerStats>();
        FindCamera();
    }

    private void FindCamera()
    {
        //Lo que cambié fue: Agregué cullLayer y agregué una sobrecarga en el init del CamRotationController para cullear la máscara (° ͜ʖ°)
        int cullLayer = default(int);
        if (GameManager.screenDivided)
        {
            if (this.gameObject.name == "Player1")
            {
                _cam = GameObject.Find("CameraPlayer1").transform;
                cullLayer = Utilities.IntLayers.VISIBLETOP1;
            }
            else if (this.gameObject.name == "Player2")
            {
                _cam = GameObject.Find("CameraPlayer2").transform;
                cullLayer = Utilities.IntLayers.VISIBLETOP2;
            }
        }
        else _cam = GameObject.Find("CameraContainer").transform;

        if (cullLayer != default(int)) _cam.GetComponent<CamRotationController>().Init(transform, readJoystick, cullLayer);
        else _cam.GetComponent<CamRotationController>().Init(transform, readJoystick);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    private void CheckConditions()
    {
        if (!_isStun)
        {
            //Condition to move
            _canMove = !_pc.isAttacking && !_pm.isRolling && !_ps.isChannelingSpell && !_ps.isCastingSpell && _gameInCourse && !_pst.isDead && !_pst.isDamaged;

            //Condition to attack
            _canAttack = !_pm.isRolling && !_ps.isPhasingSpell && !_ps.isChannelingSpell && !_ps.isCastingSpell && _gameInCourse && !_pst.isDead && !_pst.isDamaged /*&& !_pc.isAttacking*/;
        }
        else
        {
            _canMove = false;
            _canAttack = false;
        }

        //Gets the movement direction
        _direction = readJoystick ? new Vector3(InputManager.instance.GetJoystickHorizontalMovement(), 0f, InputManager.instance.GetJoystickVerticalMovement())
                                  : new Vector3(InputManager.instance.GetHorizontalMovement(), 0f, InputManager.instance.GetVerticalMovement());

        _pm.Rotate(_direction, _cam);
    }

    private void OnGameFinished(params object[] paramsContainer)
    {
        _gameInCourse = false;
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        _gameInCourse = true;

        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener(GameEvents.GameFinished, OnGameFinished);
            EventManager.RemoveEventListener(CharacterEvents.Stun, OnStun);
            EventManager.RemoveEventListener(AnimationEvents.StopStun, OnStopStun);
            EventManager.RemoveEventListener(CharacterEvents.GuardBreak, OnGuardBreak);
            EventManager.RemoveEventListener(AnimationEvents.SpecialAttack, OnSpecialAttack);
            EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);
        }
        else FindCamera();
    }

    #region Movement
    /// <summary>Checks if the character should move</summary>
    private void CheckMove()
    {
        if (_canMove)
            _pm.Move(_direction);
    }

    /// <summary>Makes the character run and roll</summary>
    private void CheckRollAndRun()
    {
        if (_canMove && !_checkingRoll)
        {
            if (readJoystick)
            {
                if (InputManager.instance.GetJoystickDodge())
                {
                    if (_pc.isBlocking) _pc.Block(true);
                    else
                    {
                        _checkingRoll = true;
                        StartCoroutine(CheckEvade(_dodgeTime));
                    }
                }
                else if (!InputManager.instance.GetJoystickRun())
                {
                    if (!_pm.sprintAvailable) _pm.StopRun();
                    if (_pc.isBlocking) _pc.Block(false);
                }
            }
            else
            {
                if (InputManager.instance.GetDodge())
                {
                    if (_pc.isBlocking) _pc.Block(true);
                    else
                    {
                        _checkingRoll = true;
                        StartCoroutine(CheckEvade(_dodgeTime));
                    }
                }
                else if (!InputManager.instance.GetRun())
                {
                    if (!_pm.sprintAvailable) _pm.StopRun();
                    if (_pc.isBlocking) _pc.Block(false);
                }
            }
        }
    }
    #endregion

    #region Attacks
    private void CheckAttacks()
    {
        if (_canAttack)
        {
            if (readJoystick)
            {
                if (InputManager.instance.GetJoystickLightAttack())
                    _pc.DoLightAttack();
                else if (InputManager.instance.GetJoystickHeavyAttack())
                    _pc.DoHeavyAttack();
            }
            else
            {
                if (InputManager.instance.GetLightAttack())
                    _pc.DoLightAttack();
                else if (InputManager.instance.GetHeavyAttack())
                    _pc.DoHeavyAttack();
            }
        }

        if (_canMove && _canBlock)
        {
            if (readJoystick)
            {
                if (!_pc.isBlocking && !_pm.isRunning && InputManager.instance.GetJoystickBlocking())
                    _pc.Block(false);
                else if (_pc.isBlocking && !InputManager.instance.GetJoystickBlocking())
                    _pc.StopBlock();
            }
            else
            {
                if (!_pc.isBlocking && !_pm.isRunning && InputManager.instance.GetBlocking())
                    _pc.Block(false);
                else if (_pc.isBlocking && !InputManager.instance.GetBlocking())
                    _pc.StopBlock();
            }
        }
    }
    #endregion

    #region Skills
    private void CheckSkills()
    {
        if (_canMove)
        {
            if (readJoystick)
            {
                if (InputManager.instance.GetJoystickEnviromentSkill())
                    _ps.EnvironmentalSkill();
                else if (InputManager.instance.GetJoystickClassSkill())
                    _ps.ClassSkill();
                else if (!_ps.isPulling && InputManager.instance.GetJoystickUniversalSkill())
                    _ps.UniversalSkill();
                else if (_pm.CheckEnemyDistance(_cam) && InputManager.instance.GetJoystickMovementSkill())
                    _ps.MovementSkill(_pst.mana);
                else if (InputManager.instance.GetJoystickUseSkill())
                    _ps.UseSkill(_pst.mana);
            }
            else
            {
                if (InputManager.instance.GetEnviromentSkill())
                    _ps.EnvironmentalSkill();
                else if (InputManager.instance.GetClassSkill())
                    _ps.ClassSkill();
                else if (InputManager.instance.GetUniversalSkill())
                    _ps.UniversalSkill();
                else if (_pm.CheckEnemyDistance(_cam) && InputManager.instance.GetMovementSkill())
                    _ps.MovementSkill(_pst.mana);
                else if (InputManager.instance.GetUseSkill())
                    _ps.UseSkill(_pst.mana);
            }
        }
    }
    #endregion

    #region Events
    void OnTransition(object[] paramsContainer)
    {
        _gameInCourse = (bool)paramsContainer[0];
    }

    void OnDoubleEdgedScalesCasted(object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == this.gameObject.name)
        {
            EventManager.DispatchEvent(ParticleEvents.GuardBreakParticle, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), (float)paramsContainer[1] });

            _canBlock = false;
            Invoke("BlockEnable", (float)paramsContainer[1]);
        }
    }

    void OnSpecialAttack(object[] paramsContainer)
    {
        if (gameObject.name == (string)paramsContainer[0])
        {
            _pc.StopBlock();
        }
    }

    private void OnStun(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] != this.gameObject.name)
        {
            EventManager.DispatchEvent(ParticleEvents.StunParticle, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), (float)paramsContainer[1] });
            EventManager.DispatchEvent(CameraEvents.StunShake, new object[] { GetCamera.transform, (float)paramsContainer[1], _canBlock });

            _isStun = true;
            Invoke("StopStun", (float)paramsContainer[1]);
        }
    }

    private void OnGuardBreak(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] != this.gameObject.name)
        {
            EventManager.DispatchEvent(ParticleEvents.GuardBreakParticle, new object[] { this.gameObject.name, transform.position, this.GetComponent<PlayerParticles>(), (float)paramsContainer[1] });

            _canBlock = false;
            Invoke("BlockEnable", (float)paramsContainer[1]);
        }
    }
    #endregion

    void BlockEnable()
    {
        _canBlock = true;
    }

    #region Coroutines
    /// <summary>Checks if the character is going to Run or going to Roll</summary>
    IEnumerator CheckEvade(float time)
    {
        var w = new WaitForSeconds(time);
        yield return w;

        if (readJoystick)
        {
            if (!InputManager.instance.GetJoystickRun() && !_pm.isRolling && !_ps.gtHasObject)
                _pm.Roll(_direction);
            else if (_pm.sprintAvailable && !_pc.isBlocking && _direction.z > 0)
                _pm.Run();
        }
        else
        {
            if (!InputManager.instance.GetRun() && !_pm.isRolling && !_ps.gtHasObject)
                _pm.Roll(_direction);
            else if (_pm.sprintAvailable && !_pc.isBlocking && _direction.z > 0)
                _pm.Run();
        }

        _checkingRoll = false;
    }

    private void OnStopStun(params object[] paramsContainer)
    {
        if ((string)paramsContainer[0] == gameObject.name)
        {
            _isStun = false;
        }
    }

    private void StopStun()
    {
        EventManager.DispatchEvent(CameraEvents.StopStunCamera, new object[] { gameObject.name, GetCamera.transform });
    }
    #endregion
}
