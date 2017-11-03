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

    [System.Obsolete("Ya no se usa más")]
    public Color destructibleColor;

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

    [System.Obsolete("Ya no se usa más")]
    private List<MarkableObject> _allMarkables;
    [System.Obsolete("Ya no se usa más")]
    private Color _originalColor = Color.white;


    Vector2 _offset;
    Quaternion _fixedLocalRot;
    CameraShake _shake;
    bool _isInTransition;

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

        var temp1 = _cam.transform.FindChild("SubCam1");
        var temp2 = _cam.transform.FindChild("SubCam2");

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
        _enemyMask = PhotonNetwork.offlineMode ? 0 << LayerMask.NameToLayer("Enemy") : 0 << LayerMask.NameToLayer("Player");
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
        //initialPosition = _cam.transform.localPosition;
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);

        _enemy = GetEnemy();
        _lockPosition = _enemy.Find("LockOnPosition");
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
        //initialPosition = _cam.transform.localPosition;
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);
        _proyectionLayer = cullLayer;
        _lockOnLayer = _proyectionLayer == 16 ? 20 : 21;

        _enemy = GetEnemy();
        _lockPosition = _enemy.Find("LockOnPosition");
        //showProjections = true;
        oldParent = transform.parent;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("ChangeStateDestuctibleProjections", ActivateProjections);
        /*EventManager.AddEventListener("DoConnect", UseProjections);
        EventManager.AddEventListener("DoNotConnect", UseProjections);
        EventManager.AddEventListener("DoDummyTest", UseProjections);
        EventManager.AddEventListener("DividedScreen", UseProjections);*/
        //EventManager.AddEventListener("BeginGame", UseProjections);
        EventManager.AddEventListener("GameFinished", OnGameFinished);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
        EventManager.AddEventListener("TransitionSmoothCameraUpdate", OnTransitionSmoothUpdate);
        EventManager.AddEventListener("StunShake", OnStun);
        EventManager.AddEventListener("StopStunCamera", OnStopStun);
        //EventManager.AddEventListener("TransitionCameraUpdate", OnTransitionCameraUpdate);
    }

    void OnTransitionSmoothUpdate(object[] paramsContainer)
    {
        smoothCamera = (bool)paramsContainer[0];
    }

    private Transform GetEnemy()
    {
        if (GameManager.screenDivided || !PhotonNetwork.offlineMode)
        {
            var enems = GameObject.FindObjectsOfType<Player1Input>();

            foreach (var enem in enems)
            {
                if (enem.transform != _character)
                    return enem.transform;
            }
        }
        else
        {
            var enems = GameObject.FindObjectsOfType<CharacterMovement>();

            foreach (var enem in enems)
            {
                if (enem.transform != _character)
                    return enem.transform;
            }
        }

        return null;
    }
    #endregion

    #region Events
    void UseProjections(object[] paramsContainer)
    {
        showProjections = (bool)paramsContainer[0];
    }

    void ActivateProjections(object[] paramsContainer)
    {
        showProjections = (bool)paramsContainer[0];
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
            EventManager.RemoveEventListener("ChangeStateDestuctibleProjections", ActivateProjections);
            /*EventManager.RemoveEventListener("DoConnect", UseProjections);
            EventManager.RemoveEventListener("DoNotConnect", UseProjections);
            EventManager.RemoveEventListener("DoDummyTest", UseProjections);
            EventManager.RemoveEventListener("DividedScreen", UseProjections);*/
            EventManager.RemoveEventListener("GameFinished", OnGameFinished);
            EventManager.RemoveEventListener("RestartRound", OnRestartRound);
            EventManager.RemoveEventListener("TransitionSmoothCameraUpdate", OnTransitionSmoothUpdate);
            EventManager.RemoveEventListener("StunShake", OnStun);
            EventManager.RemoveEventListener("StopStunCamera", OnStopStun);

        }
    }
    #endregion

    void Update()
    {
        if (showProjections) HighlightTarget();
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

        var checkVision = Physics.Raycast(_fixedCharPos, dir, dist, _enemyMask) && Physics.Raycast(transform.TransformPoint(_cam.transform.localPosition), dir2, lockOnDistance, _enemyMask);

        if (!_lockOn && Vector3.Distance(_character.position, Enemy.position) <= lockOnDistance && !checkVision)
        {
            _lockOn = true;
            EventManager.DispatchEvent("LockOnActivated", new object[] { _character.gameObject.name, _lockOn, _lockPosition, GetCamera });
        }
        else if (_lockOn)
        {
            _lockOn = false;
            EventManager.DispatchEvent("LockOnActivated", new object[] { _character.gameObject.name, _lockOn });
        }
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(_character.position, Enemy.position) > lockOnDistance)
        {
            _lockOn = false;
            EventManager.DispatchEvent("LockOnActivated", new object[] { _character.gameObject.name, _lockOn });
        }
    }

    private void LockOn()
    {
        _fixedCharPos = Enemy.position + _correctionVector;
        var direction = (_fixedCharPos - transform.position).normalized;

        direction = new Vector3(direction.x, 0f, direction.z);

        if (transform.forward != direction)
            transform.forward = Vector3.Lerp(transform.forward, direction, smoothPercentage);
        CheckDistance();
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
        //Agrego que estén en la zona, mi cabe zona
        var dstruc = DestructibleObject.allObjs;

        if (dstruc != null)
        {
            List<DestructibleObject> inRangeObj = dstruc.Where(x => x != null
                                                             && x.isAlive
                                                             && x.zone == TransitionManager.instance.currentZone
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

    void MakeVisible(DestructibleObject obj, bool visible)
    {
        var wf = obj.GetComponentsInChildren<DestructibleImpactArea>().Where(x => x.gameObject.layer == _proyectionLayer).FirstOrDefault();
        if (wf != default(DestructibleImpactArea))
        {
            wf.SetVisible(visible);
        }

    }

    [System.Obsolete("Ya no se usa más")]
    private void ChangeColor(DestructibleObject obj, Color col)
    {
        var renders = obj.GetComponentsInChildren<Renderer>();

        foreach (var rend in renders)
        {
            var mat = rend.material;
            mat.color = col;
            rend.material = mat;
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

    #region Transition
    /// <summary>
    /// 0 - Is Start?
    /// </summary>
    /// <param name="paramsContainer"></param>
    void OnTransitionCameraUpdate(object[] paramsContainer)
    {
        var start = (bool)paramsContainer[0];

        Camera subCam;
        Vector2 originalPosCam1 = GetCamera.rect.position;
        Vector2 originalPosCam2;

        var lerpTime = .2f;

        if (_proyectionLayer == Utilities.IntLayers.VISIBLETOP1)
        {
            subCam = GetCamera.transform.FindChild("SubCam1").GetComponent<Camera>();
            originalPosCam2 = subCam.rect.position;

            if (start)
            {
                _offset = new Vector2(-.5f, 1f);

                StartCoroutine(LerpRect(GetCamera, originalPosCam1, _offset, lerpTime));
                StartCoroutine(LerpRect(subCam, originalPosCam2, _offset, lerpTime));
            }
            else
            {
                _offset = new Vector2(0f, 1f);

                StartCoroutine(LerpRect(GetCamera, _offset, originalPosCam1, lerpTime));
                StartCoroutine(LerpRect(subCam, _offset, originalPosCam2, lerpTime));
            }
        }
        else
        {
            subCam = GetCamera.transform.FindChild("SubCam2").GetComponent<Camera>();
            originalPosCam2 = subCam.rect.position;

            if (start)
            {
                _offset = new Vector2(1f, 0f);

                StartCoroutine(LerpRect(GetCamera, originalPosCam1, _offset, lerpTime));
                StartCoroutine(LerpRect(subCam, originalPosCam2, _offset, lerpTime));
            }
            else
            {
                _offset = new Vector2(.5f, 0f);

                StartCoroutine(LerpRect(GetCamera, _offset, originalPosCam1, lerpTime));
                StartCoroutine(LerpRect(subCam, _offset, originalPosCam2, lerpTime));
            }

        }

        /*StartCoroutine(TransitionCameraUpdate(GetCamera, .5f));
        StartCoroutine(TransitionCameraUpdate(subCam, .5f));*/
    }

    /// Camera's offset in screen coordinates (animate this using your favourite method). 
    /// Zero means no effect. Axes may be swapped from what you expect. 
    /// Experiment with values between -1 and 1. public Vector2 offset;

    void MoveCameraRect(Camera cam)
    {
        var r = new Rect(0f, 0f, 1f, 1f);
        var alignFactor = Vector2.one;

        if (_offset.y >= 0f)
        {
            // Sliding down
            r.height = 1f - _offset.y;
            alignFactor.y = 1f;
        }
        else
        {
            // Sliding up
            r.y = -_offset.y;
            r.height = 1f + _offset.y;
            alignFactor.y = -1f;
        }

        if (_offset.x >= 0f)
        {
            // Sliding right
            r.width = 1f - _offset.x;
            alignFactor.x = 1f;
        }
        else
        {
            // Sliding left
            r.x = -_offset.x;
            r.width = 1f + _offset.x;
            alignFactor.x = -1f;
        }

        // Avoid division by zero
        if (r.width == 0f)
        {
            r.width = 0.001f;
        }
        if (r.height == 0f)
        {
            r.height = 0.001f;
        }

        // Set the camera's render rectangle to r, but use the normal projection matrix
        // This works around Unity modifying the projection matrix to correct for the aspect ratio
        // (which is normally desirable behaviour, but interferes with this effect)
        cam.rect = new Rect(0, 0, 1, 1);
        cam.ResetProjectionMatrix();
        var m = cam.projectionMatrix;
        cam.rect = r;

        // The above has caused the scene render to be squashed into the rectangle r.
        // Apply a scale factor to un-squash it.
        // The translation factor aligns the top of the scene to the top of the view
        // (without this, the view is of the middle of the scene)
        var m2 = Matrix4x4.TRS(
            new Vector3(alignFactor.x * (-1 / r.width + 1), alignFactor.y * (-1 / r.height + 1), 0),
            Quaternion.identity,
            new Vector3(1 / r.width, 1 / r.height, 1));

        cam.projectionMatrix = m2 * m;
    }

    void MoveCameraRect(Camera cam, Vector2 offst)
    {
        var r = new Rect(0f, 0f, 1f, .5f);
        var alignFactor = Vector2.one;

        if (offst.y >= 0f)
        {
            // Sliding down
            r.height = 1f - offst.y;
            alignFactor.y = 1f;
        }
        else
        {
            // Sliding up
            r.y = -offst.y;
            r.height = 1f + offst.y;
            alignFactor.y = -1f;
        }

        if (offst.x >= 0f)
        {
            // Sliding right
            r.width = 1f - offst.x;
            alignFactor.x = 1f;
        }
        else
        {
            // Sliding left
            r.x = -offst.x;
            r.width = 1f + offst.x;
            alignFactor.x = -1f;
        }

        // Avoid division by zero
        if (r.width == 0f)
        {
            r.width = 0.001f;
        }
        if (r.height == 0f)
        {
            r.height = 0.001f;
        }

        // Set the camera's render rectangle to r, but use the normal projection matrix
        // This works around Unity modifying the projection matrix to correct for the aspect ratio
        // (which is normally desirable behaviour, but interferes with this effect)
        cam.rect = new Rect(0, 0, 1f, .5f);
        //cam.ResetProjectionMatrix();
        //var m = cam.projectionMatrix;
        cam.rect = r;

        // The above has caused the scene render to be squashed into the rectangle r.
        // Apply a scale factor to un-squash it.
        // The translation factor aligns the top of the scene to the top of the view
        // (without this, the view is of the middle of the scene)
        /*var m2 = Matrix4x4.TRS(
            new Vector3(alignFactor.x * (-1f / r.width + .5f), alignFactor.y * (-1f / r.height + 1f), 0),
            Quaternion.identity,
            new Vector3(1f / r.width, 1.5f / r.height, 1f));

        cam.projectionMatrix = m2 * m;*/
    }

    #endregion

    #region Coroutines
    IEnumerator LerpRectPosition(Camera cam, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            MoveCameraRect(cam);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TransitionCameraUpdate(Camera cam, float maxTime)
    {
        var i = 0f;
        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            var offst = Vector2.Lerp(_offset, new Vector2(-_offset.x, 0), i);
            MoveCameraRect(cam, offst);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LerpRect(Camera cam, Vector3 startPos, Vector3 endPos, float maxTime)
    {
        var i = 0f;

        while (i <= 1)
        {
            i += Time.deltaTime / maxTime;
            var rkt = cam.rect;
            var vkt = Vector3.Lerp(startPos, endPos, i);
            rkt.Set(vkt.x, vkt.y, 1, .5f);
            cam.rect = rkt;
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

}

[System.Obsolete("Ya no se usa más")]
public class MarkableObject
{
    public float distance;
    public float angle;
    public DestructibleObject target;

    public MarkableObject(float dist, float ang, DestructibleObject targ)
    {
        distance = dist;
        angle = ang;
        target = targ;
    }
}
