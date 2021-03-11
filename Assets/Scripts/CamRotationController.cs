using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CamRotationController : MonoBehaviour
{
    #region Variables
    public float minimumY = -45f;
    public float maximumY = 15f;
    public float rotateSpeed = 0.9f;
    public float smoothPercentage = 0.6f;
    public float lockOnDistance;
    public float destructibleDistance = 15f;
    public float sensivity;
    public bool showProjections;
    public bool smoothCamera = true;
    public Vector3 initialPosition;

    int _proyectionLayer;
    int _lockOnLayer;

    private Transform _character;
    private Transform _lockPosition;
    private Transform _enemy;
    private float _rotationY;
    private float _rotationX;
    private Vector3 _fixedCharPos;
    private Vector3 _correctionVector;
    private Vector3 _direction;
    private float _frontDistance;
    private float _backDistance;
    private float _angleVision = 30f;
    private float _instHorizontal;
    private float _instVertical;
    private bool _gameInCourse = true;
    private bool _lockOn = false;
    private bool _readJoystick;
    private LayerMask _mask;
    private LayerMask _enemyMask;
    private Camera _cam;
    private RaycastHit _hit;
    private DestructibleObject _currentTarget;

    Camera _subCam;
    Transform oldParent;

    Vector2 _offset;
    Quaternion _fixedLocalRot;
    CameraShake _shake;
    bool _isInTransition;
    string _owner;

    public DestructibleObject CurrentTarget
    {
        get { return _currentTarget; }
    }

    public Quaternion FixedRotation
    {
        get { return _fixedLocalRot; }
        set { _fixedLocalRot = value; }
    }

    public Transform Enemy
    {
        get { return _enemy; }
    }

    public Camera GetCamera
    {
        get { return _cam; }
    }

    public Camera GetSubCamera
    {
        get { return _subCam; }
    }

    public float AngleVision
    {
        get { return _angleVision; }
    }
    #endregion

    void Start()
    {
        GetComponents();
        AddEvents();
        gameObject.AddComponent(typeof(CameraShake));
        _shake = GetComponent<CameraShake>();
    }

    #region Initialization
    private void GetComponents()
    {
        _cam = GetComponentInChildren<Camera>();

        var temp1 = _cam.transform.Find("SubCam1");
        var temp2 = _cam.transform.Find("SubCam2");

        var temp = temp1 != null ? temp1 : temp2;

        //_subCam = temp.GetComponent<Camera>();

        _mask = ~(1 << LayerMask.NameToLayer("Player")
                | 1 << LayerMask.NameToLayer("Enemy")
                | 1 << LayerMask.NameToLayer("Floor")
                | 1 << LayerMask.NameToLayer("HitBox")
                | 1 << LayerMask.NameToLayer("PlayerCollider")
                | 1 << Utilities.IntLayers.VISIBLETOP1
                | 1 << Utilities.IntLayers.VISIBLETOP2
                | 1 << Utilities.IntLayers.VISIBLETOBOTH
                | 1 << LayerMask.NameToLayer("WeaponCollider")
                 );
        _enemyMask = 0 << LayerMask.NameToLayer("Enemy");
        _correctionVector = new Vector3(0f, 1f, 0f);
        lockOnDistance = lockOnDistance == 0f ? 10f : lockOnDistance;

        _enemy = GetEnemy();
    }

    public void Init(Transform charac, bool readJoystick)
    {
        _character = charac;
        _readJoystick = readJoystick;
        if (_readJoystick) sensivity = 4;
        else sensivity = 0.1f;
        transform.position = _character.position;
        transform.rotation = _character.rotation;
        if (_cam == null) _cam = GetComponentInChildren<Camera>();
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);
        _lockOnLayer = _proyectionLayer == 16 ? 20 : 21;

        _enemy = GetEnemy();
        _lockPosition = _enemy.Find("LockOnPosition");
        oldParent = transform.parent;
        _owner = charac.name;
    }

    public void Init(Transform charac, bool readJoystick, int cullLayer)
    {
        _character = charac;
        _readJoystick = readJoystick;
        if (_readJoystick) sensivity = 4;
        else sensivity = 0.1f;
        transform.position = _character.position;
        transform.rotation = _character.rotation;
        if (_cam == null) _cam = GetComponentInChildren<Camera>();
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);
        _proyectionLayer = cullLayer;
        _lockOnLayer = _proyectionLayer == 16 ? 20 : 21;

        _enemy = GetEnemy();
        _lockPosition = _enemy.Find("LockOnPosition");
        oldParent = transform.parent;
        _owner = charac.name;

    }

    private void AddEvents()
    {
        EventManager.AddEventListener(SkillEvents.ChangeStateDestuctibleProjections, ActivateProjections);
        EventManager.AddEventListener(GameEvents.GameFinished, OnGameFinished);
        EventManager.AddEventListener(GameEvents.RestartRound, OnRestartRound);
        EventManager.AddEventListener(CameraEvents.StunShake, OnStun);
        EventManager.AddEventListener(CameraEvents.StopStunCamera, OnStopStun);
        EventManager.AddEventListener(CameraEvents.StartBlinkFeedback, OnActivateBlink);
        EventManager.AddEventListener(ParticleEvents.ToxicDamageParticle, OnToxicDamage);
    }

    void OnToxicDamage(object[] paramsContainer)
    {
        if (_owner == (string)paramsContainer[0])
        {
            var duration = 3;
            /*var profile = GetCamera.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
            var postProcess = Resources.Load("PostProcess/ToxinePostProcess") as UnityEngine.PostProcessing.PostProcessingProfile;
            profile.profile = postProcess;
            StartToxine(postProcess, duration);*/
        }
    }

    /*void StartToxine(UnityEngine.PostProcessing.PostProcessingProfile postProcess, float maxTime)
    {
        StartCoroutine(PingPongFloat(postProcess.vignette.settings.intensity, 0, 1, maxTime));
    }*/

    IEnumerator LerpFloat(float value, float startValue, float endValue, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            value = Mathf.Lerp(startValue, endValue, i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PingPongFloat(float value, float startValue, float endValue, float maxTime)
    {
        var i = 0f;
        var timeFrame = maxTime / 2;

        while (i <= 1)
        {
            i += Time.deltaTime / timeFrame;
            value = Mathf.Lerp(startValue, endValue, i);
            yield return new WaitForEndOfFrame();
        }

        i = 0;

        while (i <= 1)
        {
            i += Time.deltaTime / timeFrame;
            value = Mathf.Lerp(endValue, startValue, i);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnTransitionSmoothUpdate(object[] paramsContainer)
    {
        smoothCamera = (bool)paramsContainer[0];
    }

    private Transform GetEnemy()
    {
        if (GameManager.screenDivided)
        {
            var enems = GameObject.FindObjectsOfType<PlayerInput>();

            foreach (var enem in enems)
            {
                if (enem.transform != _character)
                    return enem.transform;
            }
        }
        else
        {
            return null;
        }

        return null;
    }
    #endregion

    #region Events
    void UseProjections(object[] paramsContainer)
    {
        showProjections = (bool)paramsContainer[0];
    }

    /// <summary>
    /// 0 - showProyections - bool
    /// 1 - senderName - string
    /// </summary>
    /// <param name="paramsContainer"></param>
    void ActivateProjections(object[] paramsContainer)
    {
        var sender = (string)paramsContainer[1];
        var activate = (bool)paramsContainer[0];

        if (_owner == sender)
        {
            showProjections = activate;
        }
    }

    private void OnGameFinished(params object[] paramsContainer)
    {
        showProjections = false;
        _gameInCourse = false;
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        _gameInCourse = true;
        showProjections = true;

        if ((bool)paramsContainer[0])
        {
            EventManager.RemoveEventListener(SkillEvents.ChangeStateDestuctibleProjections, ActivateProjections);
            EventManager.RemoveEventListener(GameEvents.GameFinished, OnGameFinished);
            EventManager.RemoveEventListener(GameEvents.RestartRound, OnRestartRound);
            EventManager.RemoveEventListener(CameraEvents.StunShake, OnStun);
            EventManager.RemoveEventListener(CameraEvents.StopStunCamera, OnStopStun);
            EventManager.RemoveEventListener(CameraEvents.StartBlinkFeedback, OnActivateBlink);

        }
    }
    #endregion

    void Update()
    {
        HighlightTarget();
    }

    void FixedUpdate()
    {
        if (_gameInCourse) MoveCamera();
    }

    void LateUpdate()
    {
        if (_gameInCourse)
        {
            ClippingBehaviour();
            if (!_readJoystick && InputManager.instance.GetLockOn()) CamLock();
            else if (_readJoystick && InputManager.instance.GetJoystickLockOn()) CamLock();
        }
    }

    void OnActivateBlink(object[] paramsContainer)
    {
        if ((CamRotationController)paramsContainer[0] == this)
        {
            BlinkFeedback();
        }
    }

    void BlinkFeedback()
    {
        var startFov = GetCamera.fieldOfView;
        var endFov = GetCamera.fieldOfView * 2;
        StartCoroutine(LerpCameraFOV(startFov, endFov, .3f));
    }

    #region Movement
    void MoveCamera()
    {
        if (_character != null)
        {
            if (transform.position != _character.position)
            {
                if (smoothCamera)
                    transform.position = Vector3.Lerp(transform.position, _character.position, smoothPercentage);
                else
                    transform.position = _character.position;
            }

            if (!_lockOn)
            {
                if (_readJoystick)
                {
                    _instHorizontal = InputManager.instance.GetJoystickHorizontalCamera();
                    _instVertical = InputManager.instance.GetJoystickVerticalCamera();

                    if (_instHorizontal != 0 || _instVertical != 0)
                    {
                        _rotationY += _instVertical * rotateSpeed * sensivity;
                        _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);
                        _rotationX = _instHorizontal * rotateSpeed * sensivity + transform.eulerAngles.y;

                        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
                    }
                }
                else
                {
                    _instHorizontal = InputManager.instance.GetHorizontalCamera();
                    _instVertical = InputManager.instance.GetVerticalCamera();

                    if (_instHorizontal != 0 || _instVertical != 0)
                    {
                        _rotationY += -_instVertical * rotateSpeed * sensivity;
                        _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);
                        _rotationX = _instHorizontal * rotateSpeed * sensivity + transform.eulerAngles.y;

                        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
                    }
                }
            }
            else LockOn();
        }
    }
    #endregion

    #region Lock
    private void CamLock()
    {
        var dir = (Enemy.position - _character.position).normalized;
        var dir2 = (Enemy.position - transform.TransformPoint(_cam.transform.localPosition)).normalized;
        var dist = lockOnDistance - Vector3.Distance(transform.TransformPoint(_cam.transform.localPosition), _character.position);
        _fixedCharPos = _character.position + _correctionVector;

        //Triplique la distancia del raycast teniendo en referencia la distancia de condicion que teniamos antes, porque la verdad no se cuanta distancia ponerle para este escenario.
        var checkVision = Physics.Raycast(_fixedCharPos, dir, dist, _enemyMask) && Physics.Raycast(transform.TransformPoint(_cam.transform.localPosition), dir2, lockOnDistance * 3, _enemyMask);

        //Vector3.Distance(_character.position, Enemy.position) <= lockOnDistance
        //Esa es la condición de distancia, la dejo aca por si decidimos volver a activarla.
        if (!_lockOn && !checkVision)
        {
            _lockOn = true;
            smoothCamera = false;
            EventManager.DispatchEvent(CameraEvents.LockOnActivated, new object[] { _character.gameObject.name, _lockOn, _lockPosition, GetCamera });
        }
        else if (_lockOn)
        {
            _lockOn = false;
            smoothCamera = true;
            EventManager.DispatchEvent(CameraEvents.LockOnActivated, new object[] { _character.gameObject.name, _lockOn });
        }
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(_character.position, Enemy.position) > lockOnDistance)
        {
            _lockOn = false;
            EventManager.DispatchEvent(CameraEvents.LockOnActivated, new object[] { _character.gameObject.name, _lockOn });
        }
    }

    private void LockOn()
    {
        _fixedCharPos = Enemy.Find("LockOnPosition").position + _correctionVector;
        var direction = (_fixedCharPos - transform.position).normalized;

        direction = new Vector3(direction.x, 0f, direction.z);

        if (transform.forward != direction)
        {
            if (smoothCamera) transform.forward = Vector3.Lerp(transform.forward, direction, smoothPercentage);
            else transform.forward = direction;
        }
          
        //Quito la llamada a este método para que no se desactive el lock on si se aleja demasiado.
        //CheckDistance();
    }
    #endregion

    #region Clipping
    private void ClippingBehaviour()
    {
        if (_character != null)
        {
            _fixedCharPos = _character.position + _correctionVector;
            _direction = (_fixedCharPos - transform.TransformPoint(_cam.transform.localPosition)).normalized;
            _frontDistance = Vector3.Distance(transform.TransformPoint(_cam.transform.localPosition), _fixedCharPos);
            _backDistance = Vector3.Distance(transform.TransformPoint(initialPosition), _fixedCharPos);

            if (Physics.Raycast(transform.TransformPoint(_cam.transform.localPosition), _direction, out _hit, _frontDistance, _mask.value))
            {
                _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, transform.InverseTransformPoint(_hit.point + _direction * 0.1f), smoothPercentage);
            }
            else if (_cam.transform.localPosition != initialPosition && Physics.Raycast(_fixedCharPos, -_direction, out _hit, _backDistance, _mask.value))
            {
                _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, transform.InverseTransformPoint(_hit.point + _direction * 0.1f), smoothPercentage);
            }
            else
            {
                _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, initialPosition, smoothPercentage);
            }
        }
    }
    #endregion

    #region Highlight
    private void HighlightTarget()
    {
        if (showProjections)
        {
            //Agrego que estén en la zona, mi cabe zona
            var dstruc = DestructibleObject.allObjs;

            if (dstruc != null)
            {
                List<DestructibleObject> inRangeObj = dstruc.Where(x => x != null
                                                                 && x.isAlive
                                                                 && Vector3.Distance(x.transform.position, transform.position) <= destructibleDistance
                                                                 && x.destructibleType != DestructibleType.TRANSITION
                                                                 /*&& x.GetComponentInChildren<Renderer>().isVisible*/)
                                                        .ToList<DestructibleObject>();
                DestructibleObject closest;

                if (inRangeObj.Any())
                {
                    closest = inRangeObj[0];
                    float angle = Vector3.Angle(transform.forward, (closest.transform.position - transform.position).normalized);
                    float tempAngle;

                    foreach (var dest in inRangeObj)
                    {
                        tempAngle = Vector3.Angle(transform.forward, (dest.transform.position - transform.position).normalized);

                        if (tempAngle < angle)
                        {
                            closest = dest;
                            angle = tempAngle;
                        }
                    }

                    if (_currentTarget == null)
                    {
                        _currentTarget = closest;
                        MakeVisible(_currentTarget, true);
                    }
                    else if (closest != _currentTarget)
                    {
                        MakeVisible(_currentTarget, false);
                        _currentTarget = closest;
                        MakeVisible(_currentTarget, true);
                    }
                }
                else
                {
                    if (_currentTarget != null)
                    {
                        MakeVisible(_currentTarget, false);
                    }
                    _currentTarget = null;
                }
            }
        }
        else
        {
            if (_currentTarget != null)
            {
                MakeVisible(_currentTarget, false);
            }
            _currentTarget = null;
        }
    }

    void MakeVisible(DestructibleObject obj, bool visible)
    {
        var wf = obj.GetComponentsInChildren<DestructibleImpactArea>().Where(x => x.gameObject.layer == _proyectionLayer).FirstOrDefault();
        if (wf != default(DestructibleImpactArea))
        {
            wf.SetVisible(visible);
        }

    }
    #endregion

    #region Shake
    public void ShakeCamera(float amount, float duration)
    {
        _shake.ShakeCamera(amount, duration);
    }

    /// <summary>
    /// 0 - camTransform
    /// 1 - stunTime
    /// 2 - emparent
    /// </summary>
    /// <param name="paramsContainer"></param>
    void OnStun(object[] paramsContainer)
    {
        var emparent = (bool)paramsContainer[2];
        float amount;
        var time = (float)paramsContainer[1];
        var camTrn = (Transform)paramsContainer[0];
        if (camTrn == transform)
        {
            if (emparent)
            {
                transform.parent = transform.parent.GetComponentsInChildren<Transform>().Where(x => x.name == "Base HumanRArmCollarbone").FirstOrDefault();
                amount = .3f;
                StunFeedback(amount, time);
            }
            else
            {
                amount = 12f;
                ShakeCamera(amount, time);
            }
        }
    }

    void StunFeedback(float displacementAmount, float time)
    {
        iTween.ShakeRotation(gameObject, transform.forward * displacementAmount, time);
    }

    void OnStopStun(object[] paramsContainer)
    {
        var camTrn = (Transform)paramsContainer[1];
        if (camTrn == transform)
        {
            transform.parent = oldParent;
        }
    }
    #endregion

    #region Coroutines

    IEnumerator LerpCameraFOV(float startValue, float endValue, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / (maxTime / 2);
            var lerp = Mathf.Lerp(startValue, endValue, i);
            GetCamera.fieldOfView = lerp;
            yield return new WaitForEndOfFrame();
        }

        i = 0;

        while (i <= 1)
        {
            i += Time.deltaTime / (maxTime / 2);
            var lerp = Mathf.Lerp(endValue, startValue, i);
            GetCamera.fieldOfView = lerp;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

}
