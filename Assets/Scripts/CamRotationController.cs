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
    public Vector3 initialPosition;
    public Color destructibleColor;

    int proyectionLayer;

    private Transform _character;
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
    private bool _keepReadjusting = false;
    private bool _readJoystick;
    private Color _originalColor = Color.white;
    private LayerMask _mask;
    private LayerMask _enemyMask;
    private Camera _cam;
    private RaycastHit _hit;
    private List<MarkableObject> _allMarkables;
    private DestructibleObject _currentTarget;

    public DestructibleObject CurrentTarget
    {
        get { return _currentTarget; }
    }

	public Camera GetCamera
    {
        get { return _cam; }
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
    }

    #region Initialization
    private void GetComponents()
    {
        //Ivan: para que haces esto si cada camara tiene solo un hijo de tipo camara y ninguno se llama asi??
        _cam = GetComponentsInChildren<Camera>().Where(x => x.gameObject.name != "Cam1 (1)" && x.gameObject.name != "Cam2 (1)").First();
        _mask = ~(1 << LayerMask.NameToLayer("Player") 
                | 1 << LayerMask.NameToLayer("Enemy") 
                | 1 << LayerMask.NameToLayer("Floor") 
                | 1 << LayerMask.NameToLayer("HitBox")
                | 1 << LayerMask.NameToLayer("PlayerCollider")
                | 1 << Utilities.IntLayers.VISIBLETOP1
                | 1 << Utilities.IntLayers.VISIBLETOP2
                | 1 << Utilities.IntLayers.VISIBLETOBOTH
                 );
        _enemyMask = PhotonNetwork.offlineMode ? 0 << LayerMask.NameToLayer("Enemy") : 0 << LayerMask.NameToLayer("Player");
        _correctionVector = new Vector3(0f, 1f, 0f);
        lockOnDistance = lockOnDistance == 0f ? 10f : lockOnDistance;
    }

    public void Init(Transform charac, bool readJoystick)
    {
        _character = charac;
        _readJoystick = readJoystick;
        if (_readJoystick) sensivity *= 4;
        else sensivity = 0.1f;
        transform.position = _character.position;
        transform.rotation = _character.rotation;
        if (_cam == null) _cam = GetComponentInChildren<Camera>();
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);
    }

    public void Init(Transform charac, bool readJoystick, int cullLayer)
    {
        _character = charac;
        _readJoystick = readJoystick;
        if (_readJoystick) sensivity *= 4;
        else sensivity = 0.1f;
        transform.position = _character.position;
        transform.rotation = _character.rotation;
        if (_cam == null) _cam = GetComponentInChildren<Camera>();
        _cam.transform.localPosition = transform.InverseTransformPoint(initialPosition);
        proyectionLayer = cullLayer;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("ChangeStateDestuctibleProjections", ActivateProjections);
        EventManager.AddEventListener("DoConnect", UseProjections);
        EventManager.AddEventListener("DoNotConnect", UseProjections);
        EventManager.AddEventListener("DoDummyTest", UseProjections);
        EventManager.AddEventListener("DividedScreen", UseProjections);
        EventManager.AddEventListener("GameFinished", OnGameFinished);
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
        _gameInCourse = false;
    }
    #endregion

    void Update()
    {
        if(showProjections) HighlightTarget();
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
                transform.position = Vector3.Lerp(transform.position, _character.position, smoothPercentage);

            ReLocateCamera();

            if (_readJoystick)
            {
                _instHorizontal = InputManager.instance.GetJoystickHorizontalCamera();
                _instVertical = InputManager.instance.GetJoystickVerticalCamera();
                
                if (!_lockOn && (_instHorizontal != 0 || _instVertical != 0))
                {
                    _rotationY += -_instVertical * rotateSpeed * sensivity;
                    _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);
                    _rotationX = _instHorizontal * rotateSpeed * sensivity + transform.eulerAngles.y;
                    
                    transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
                }
            }
            else
            {
                _instHorizontal = InputManager.instance.GetHorizontalCamera();
                _instVertical = InputManager.instance.GetVerticalCamera();

                if (!_lockOn && (_instHorizontal != 0 || _instVertical != 0))
                {
                    _rotationY += -_instVertical * rotateSpeed * sensivity;
                    _rotationY = Mathf.Clamp(_rotationY, minimumY, maximumY);
                    _rotationX = _instHorizontal * rotateSpeed * sensivity + transform.eulerAngles.y;

                    transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0);
                }
            }
        }
    }
    #endregion

    #region Lock
    private void CamLock()
    {
        if (_enemy == null)
        {
            if (GameManager.screenDivided || !PhotonNetwork.offlineMode)
            {
                var enems = GameObject.FindObjectsOfType<Player1Input>();

                foreach (var enem in enems)
                {
                    if (enem.transform != _character) _enemy = enem.transform;
                }
            }
            else
            {
                var enems = GameObject.FindObjectsOfType<CharacterMovement>();

                foreach (var enem in enems)
                {
                    if (enem.transform != _character) _enemy = enem.transform;
                }
            }
            
        }

        var dir = (_enemy.position - _character.position).normalized;
        var dir2 = (_enemy.position - transform.TransformPoint(_cam.transform.localPosition)).normalized;
        var dist = lockOnDistance - Vector3.Distance(transform.TransformPoint(_cam.transform.localPosition), _character.position);
        _fixedCharPos = _character.position + _correctionVector;

        var checkVision = Physics.Raycast(_fixedCharPos, dir, dist, _enemyMask) && Physics.Raycast(transform.TransformPoint(_cam.transform.localPosition), dir2, lockOnDistance, _enemyMask);

        if (!_lockOn && Vector3.Distance(_character.position, _enemy.position) <= lockOnDistance && !checkVision)
            _lockOn = true;
        else if (_lockOn)
        {
            _lockOn = false;
            _keepReadjusting = true;
        }
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(_character.position, _enemy.position) > lockOnDistance)
        {
            _lockOn = false;
            _keepReadjusting = true;
        }
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

    private void ReLocateCamera()
    {
        if (_lockOn)
        {
            _fixedCharPos = _enemy.position + _correctionVector;
            var direction = (_fixedCharPos - transform.position).normalized;

            if (transform.forward != direction)
            {
                transform.forward = Vector3.Lerp(transform.forward, direction, 0.6f);
                transform.forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
            }

            var direction2 = (_fixedCharPos - transform.TransformPoint(_cam.transform.localPosition)).normalized;

            if (_cam.transform.forward != direction2)
                _cam.transform.forward = Vector3.Lerp(_cam.transform.forward, direction2, 0.6f);

            CheckDistance();
        }
        else if(_keepReadjusting)
        {
            if (transform.forward != _character.forward || _cam.transform.forward != _character.forward)
            {
                transform.forward = Vector3.Lerp(transform.forward, _character.forward, 0.6f);
                _cam.transform.forward = Vector3.Lerp(_cam.transform.forward, _character.forward, 0.6f);
            }
            else _keepReadjusting = false;   
        }
    }
    #endregion

    #region Highlight
    private void HighlightTarget()
    {
        #region Cambios Iván 31/8
        //Agrego que no sean del tipo Transition
        List<DestructibleObject> inRangeObj = DestructibleObject.allObjs.Where(x => x.isAlive 
                                                                               && Vector3.Distance(x.transform.position, transform.position) <= destructibleDistance
                                                                               && x.destructibleType != DestructibleType.TRANSITION
                                                                               && x.GetComponentInChildren<Renderer>().isVisible)
                                                                        .ToList<DestructibleObject>();
        #endregion
        DestructibleObject closest;
        if (inRangeObj.Count() > 0)
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

            if (closest != _currentTarget)
            {
                MakeVisible(_currentTarget, false);
                _currentTarget = closest;
                MakeVisible(_currentTarget, true);
            }
        }
    }

    void MakeVisible(DestructibleObject obj, bool visible)
    {
        var wf = obj.GetComponentsInChildren<DestructibleImpactArea>().Where(x => x.gameObject.layer == proyectionLayer).FirstOrDefault();
        if (wf != default(DestructibleImpactArea))
        {
            wf.SetVisible(visible);
        }
        
    }

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
    
}

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
