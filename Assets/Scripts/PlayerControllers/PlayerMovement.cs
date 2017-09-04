using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Photon.MonoBehaviour {

    #region Variables
    private Rigidbody _rigid;
    private Transform _enemy;
    private float _originalWalkSpeed;
    private float _originalRunSpeed;

    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float speedMult;
    [HideInInspector]
    public bool isEvading;
    [HideInInspector]
    public bool runForward;
    [HideInInspector]
    public bool runRight;
    [HideInInspector]
    public bool runLeft;
    [HideInInspector]
    public bool walkForward;
    [HideInInspector]
    public bool walkRight;
    [HideInInspector]
    public bool walkLeft;
    [HideInInspector]
    public bool sprintForward;
    [HideInInspector]
    public bool sprintRight;
    [HideInInspector]
    public bool sprintLeft;
    [HideInInspector]
    public bool isRunning;
    [HideInInspector]
    public bool sprintAvailable;
    [HideInInspector]
    public bool isRolling;
    
    public Transform Enemy { get { return _enemy; } }
    #endregion

    #region Initialization
    private void InitializeVariables()
    {
        _rigid = GetComponent<Rigidbody>();
        _enemy = GetEnemy();

        _originalWalkSpeed = 350f;
        _originalRunSpeed = _originalWalkSpeed * 1.8f;
        speed = _originalWalkSpeed;

        speedMult = 1f;
        runForward = false;
        runRight = false;
        runLeft = false;
        isRunning = false;
        sprintAvailable = true;
        isRolling = false;
        isEvading = false;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("Attack", OnAttack);
        EventManager.AddEventListener("CharacterDamaged", OnCharacterDamaged);
    }
    #endregion

    void Start()
    {
        InitializeVariables();
        AddEvents();
    }

    #region Actions
    /// <summary>Moves the character in a given direction</summary>
    public void Move(Vector3 direction)
    {
        if (isRunning && direction.z <= 0)
        {
            speed = _originalWalkSpeed;
            isRunning = false;
        }

        if (direction != Vector3.zero)
            _rigid.velocity = new Vector3(transform.TransformDirection(direction).x * speed * Time.deltaTime, _rigid.velocity.y, transform.TransformDirection(direction).z * speed * Time.deltaTime);

        //Updates variables for the animator
        SetMovementStats(direction);
    }

    /// <summary>Sets the rotation of the character using the camera</summary>
    public void Rotate(Vector3 direction, Transform cam)
    {
        if(direction != Vector3.zero && !isEvading) transform.forward = new Vector3(cam.forward.x, 0, cam.forward.z);
    }

    /// <summary>Makes the character roll</summary>
    public void Roll(Vector3 direction)
    {
        isEvading = true;
        if (direction != Vector3.zero) transform.forward = new Vector3(transform.TransformDirection(direction).x, 0f, transform.TransformDirection(direction).z);
        isRolling = true;
        EventManager.DispatchEvent("RollingAnimation", new object[] { this.gameObject.name, isRolling });
        if (!PhotonNetwork.offlineMode) photonView.RPC("SetRollingOn", PhotonTargets.All);
    }

    /// <summary>Makes the character run</summary>
    public void Run()
    {
        EventManager.DispatchEvent("ActivateRunParticle", new object[] { this.gameObject.name, true });
        isRunning = true;
        sprintAvailable = false;
        speed = _originalRunSpeed;
    }

    /// <summary>Makes the character walk</summary>
    public void StopRun()
    {
        EventManager.DispatchEvent("ActivateRunParticle", new object[] { this.gameObject.name, false });
        isRunning = false;
        sprintAvailable = true;
        speed = _originalWalkSpeed;
    }

    #region Animation Variables
    /// <summary>Sets variables used by the animator</summary>
    private void SetMovementStats(Vector3 direction)
    {
        if (direction.z != 0)
        {
            if ((direction.z > 0 && direction.z <= 0.5) || (direction.z < 0 && direction.z >= -0.5))
            {
                if (direction.x > 0 && direction.x <= 0.5)
                {
                    if (isRunning)
                    {
                        walkRight = false;
                        sprintRight = true;
                    }
                    else
                    {
                        walkRight = true;
                        sprintRight = false;
                    }

                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    runLeft = false;
                    sprintForward = false;
                    sprintLeft = false;
                }
                else if (direction.x < 0 && direction.x >= -0.5)
                {
                    if (isRunning)
                    {
                        walkLeft = false;
                        sprintLeft = true;
                    }
                    else
                    {
                        walkLeft = true;
                        sprintLeft = false;
                    }
                    walkRight = false;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    runLeft = false;
                    sprintForward = false;
                    sprintRight = false;
                }
                else
                {
                    if (isRunning)
                    {
                        walkForward = false;
                        sprintForward = true;
                    }
                    else
                    {
                        walkForward = true;
                        sprintForward = false;
                    }
                    walkRight = false;
                    walkLeft = false;
                    runForward = false;
                    runRight = false;
                    runLeft = false;
                    sprintRight = false;
                    sprintLeft = false;
                }
            }
            else if ((direction.z > 0 && direction.z > 0.5) || (direction.z < 0 && direction.z < -0.5))
            {
                if (direction.x > 0 && direction.x > 0.5)
                {
                    if (isRunning)
                    {
                        runRight = false;
                        sprintRight = true;
                    }
                    else
                    {
                        runRight = true;
                        sprintRight = false;
                    }
                    walkRight = false;
                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runLeft = false;
                    sprintForward = false;
                    sprintLeft = false;
                }
                else if (direction.x < 0 && direction.x < -0.5)
                {
                    if (isRunning)
                    {
                        runLeft = false;
                        sprintLeft = true;
                    }
                    else
                    {
                        runLeft = true;
                        sprintLeft = false;
                    }
                    walkRight = false;
                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    sprintForward = false;
                    sprintRight = false;
                }
                else
                {
                    if (isRunning)
                    {
                        runForward = false;
                        sprintForward = true;
                    }
                    else
                    {
                        runForward = true;
                        sprintForward = false;
                    }
                    walkRight = false;
                    walkLeft = false;
                    walkForward = false;
                    runRight = false;
                    runLeft = false;
                    sprintRight = false;
                    sprintLeft = false;
                }
            }
        }
        else if (direction.x != 0)
        {
            if (direction.x > 0)
            {
                if (direction.x <= 0.5)
                {
                    walkRight = true;
                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    runLeft = false;
                }
                else
                {
                    walkRight = false;
                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runRight = true;
                    runLeft = false;
                }

            }
            else if (direction.x < 0)
            {
                if (direction.x >= -0.5)
                {
                    walkRight = false;
                    walkLeft = true;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    runLeft = false;
                }
                else
                {
                    walkRight = false;
                    walkLeft = false;
                    walkForward = false;
                    runForward = false;
                    runRight = false;
                    runLeft = true;
                    sprintForward = false;
                    sprintRight = false;
                    sprintLeft = false;
                }
            }
        }
        else
        {
            walkRight = false;
            walkLeft = false;
            walkForward = false;
            runForward = false;
            runRight = false;
            runLeft = false;
        }

        if (direction.z < 0) speedMult = -1.0f;
        else speedMult = 1.0f;
        
        EventManager.DispatchEvent("RunningAnimations", new object[] { this.gameObject.name, walkForward, walkRight, walkLeft, runForward, runRight, runLeft, sprintForward, sprintRight, sprintLeft, speedMult });
    }
    #endregion

    /// <summary>Stops animations on attack</summary>
    private void OnAttack(params object[] paramsContainer)
    {
        runForward = false;
    }
    #endregion

    #region Animation Events
    /// <summary>This is for an animation event</summary>
    private void OnRollExit()
    {
        isEvading = false;
        isRolling = false;
        EventManager.DispatchEvent("RollingAnimation", new object[] { this.gameObject.name, isRolling });
        if (!PhotonNetwork.offlineMode) photonView.RPC("SetRollingOff", PhotonTargets.All);
    }

    private void OnCharacterDamaged(params object[] paramsContainer)
    {
        if (this.gameObject.name == (string)paramsContainer[0])
            isRunning = false;
    }
    #endregion

    #region Enemy Reference
    public bool CheckEnemyDistance(Transform cam)
    {
        if (_enemy == null)  _enemy = GetEnemy();

        if (_enemy != null)
        {
            var dir = (_enemy.position - cam.position).normalized;
            var ang = Vector3.Angle(cam.forward, dir);

            return (Vector3.Distance(this.transform.position, _enemy.position) > 10f && ang <= 20) ? true : false;
        }

        return false;
    }

    private Transform GetEnemy()
    {
        var enems = GameObject.FindObjectsOfType<NetworkCharacter>();

        foreach (var enem in enems)
        {
            if (enem.transform != this.transform)
                return enem.transform;
        }
        
        return null;
    }
    #endregion

    #region Getters
    public Transform EnemyTransform
    {
        get { return _enemy == null ? GetEnemy() : _enemy; }
    }
    #endregion
}
