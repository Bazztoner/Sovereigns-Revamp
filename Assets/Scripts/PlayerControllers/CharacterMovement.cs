using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(NetworkCharacter))]
[RequireComponent(typeof(NetworkAttacks))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : Photon.MonoBehaviour
{
    #region Variables
    ///<summary>The rigidbody of the character</summary>>
    private Rigidbody _rigid;

    ///<summary>The vector that determines the direction in that the character moves</summary>>
    private Vector3 _direction = Vector3.zero;

    ///<summary>The camera inside the Camera Container</summary>>
    private Camera _camObj;

    ///<summary>The mesh to change the material of the character</summary>>
    private SkinnedMeshRenderer _mr;

    ///<summary>A vector that fixes the pivot position of the character</summary>>
    private Vector3 _fixedCharPos;

    ///<summary>Determines if the player can make an attack</summary>>
    private bool _isAttackAvailable = true; //Pasada

    ///<summary>Determines if the character is evading</summary>>
    public bool isEvading = false;//Pasada

    ///<summary>Determines if the character can move</summary>>
    private bool _canMove = true; //Pasada

    /// <summary>Determines if the game is in course</summary>
    private bool _gameInCourse = true; //Pasada

    ///<summary>The walk speed of the character</summary>>
    private float _originalWalkSpeed;//Pasada

    ///<summary>The run speed of the character</summary>>
    private float _originalRunSpeed; //Pasada

    ///<summary>The time to determine if the character is going to roll or to run</summary>>
    private float _dodgeTime = 0.15f; //Pasada

    ///<summary>The information obtain by every Raycast made</summary>>
    private RaycastHit _hit; //Sin usar
    
    ///<summary>The list with all the components observing this script</summary>>
    private List<IObserver> _observers = new List<IObserver>(); //Sin usar

    ISpell environmentalSpell; //Pasada
    ISpell classSpell; //Pasada
    ISpell pickedSpell; //Pasada
    ISpell mobilitySpell; //Pasada

    /// <summary> Checkea si GravitationalTelekinesis tiene un objeto</summary>
    public bool gtHasObject; //FIXME: ESTO ES UN WORKAROUND, BUSCAR MEJOR MANERA

    private bool _isPulling = false; //Pasada

    /// <summary>It's use to change the spell, this is just for test and it's not permanent</summary>
    private int _currentSpell = 0; //Sin usar

    //CHANGING TO PUBLIC BECAUSE EDITOR STUFF
    /// <summary>Sets the damage made by the light attack</summary>
    public int _lightAttackDamage = 12; //Pasada

    /// <summary>Sets the damage made by the heavy attack</summary>
    public int _heavyAttackDamage = 23; //Pasada

    /// <summary>Use to determine if an spell can be launch</summary>
    private bool _inSpellCooldown; //Sin usar

    public Transform enemy; //Pasada

    //Pasadas 
    #region Animation Variables
    [HideInInspector]
    public bool runForward = false;
    [HideInInspector]
    public bool runLeft = false;
    [HideInInspector]
    public bool runRight = false;
    [HideInInspector]
    public float speedMult = 1f;
    [HideInInspector]
    public bool isBlocking = false;
    [HideInInspector]
    public bool isRolling = false;
	[HideInInspector]
    public bool isDamaged = false;
    [HideInInspector]
    public bool _isDead = false;
    [HideInInspector]
    public bool x = false;
    [HideInInspector]
    public bool y = false;
    #endregion
    
    /// <summary>Determines if the character is casting an spell</summary>
    public bool isCastingSpell; //Pasada

    /// <summary>Sets the speed in which the character moves</summary>
    private float _speed; //Pasada

    /// <summary>The transform of the Camera Container</summary>
    private Transform _cam; //Pasada


    /// <summary>The particle system used during spells</summary>
    public ParticleSystem launchParticleSystem;
    public ParticleSystem chargeParticleSystem;

    [HideInInspector]
    /// <summary>Determines if the character is attacking</summary>
    public bool isAttacking = false; //Pasada

    [HideInInspector]
    /// <summary>Determines if the current attack is a light attack</summary>
    public bool isLightAttack = false; //Pasada

    [HideInInspector]
    /// <summary>Determines if the current attack is a heavy attack</summary>
    public bool isHeavyAttack = false; //Pasada
    
    /// <summary>The position from where the spells are cast</summary>
    public Transform spellPos; //Pasada

    /// <summary>The default material of the character</summary>
    public Material defaultMat; //Sin usar

    /// <summary>The material used to make the character transparent when the camera gets too close</summary>
    public Material transparentMat; //Sin usar

    public float hp;
    public int maxHp;
    public float mana;
    public int maxMana;
    public float hpRegeneration;
    public float manaRegeneration;
    //Ojo con esto Ivan, porque si van a tener get y set, las variables deberian ser privadas. Ya se que es para modificarlo por el inspector, pero no es lo ideal.
    #endregion

    public float Hp
    {
        get { return hp; }
        set
        {
            if (value > maxHp) hp = maxHp;
            else if (value <= 0)
            {
                hp = 0;
                EventManager.DispatchEvent("PlayerDeath", new object[] { PhotonNetwork.player.NickName });
            } 
            else hp = value;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            if (value > maxMana) mana = maxMana;
            else if ((value < 0)) mana = 0;
            else mana = value;
        }
    }
    

    /*
    void Start ()
    {
        GetComponents();
        InitializeVariables();
        AddEvents();
        LockCursor();
        StartCoroutine(Regeneration());
    }

    #region Initialization
    private void GetComponents()
    {
        _rigid = GetComponent<Rigidbody>();
        _cam = GameObject.Find("CameraContainer").transform;
        _camObj = _cam.GetComponentInChildren<Camera>();
        _cam.GetComponent<CamRotationController>().Init(transform);
        _mr = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void InitializeVariables()
    {
        _lightAttackDamage = _lightAttackDamage == 0 ? 6 : _lightAttackDamage;
        _heavyAttackDamage = _heavyAttackDamage == 0 ? 16 : _heavyAttackDamage;

        classSpell = new GravitationalTelekinesis();
        classSpell.Init(this);

        environmentalSpell = new AtractiveTelekinesis();
        environmentalSpell.Init(this);

        pickedSpell = new RepulsiveTelekinesis();
        pickedSpell.Init(this);

        mobilitySpell = new Blink();
        mobilitySpell.Init(this);

        _inSpellCooldown = false;

        _originalWalkSpeed = 5f;
        _originalRunSpeed = _originalWalkSpeed * 2.1f;
        _speed = _originalWalkSpeed;
    }

    private void AddEvents()
    {
        EventManager.AddEventListener("GameFinished", OnGameFinished);
        EventManager.AddEventListener("ObjectPulled", OnObjectPulled);
        EventManager.AddEventListener("SpellCasted", OnSpellCasted);
        EventManager.AddEventListener("ObjectPulling", OnObjectPulling);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    void Update ()
    {
        if (_gameInCourse)
        {
            Actions();
            CheckCameraDistance();
        }
	}

    void FixedUpdate()
    {
        if(!_isDead)
            if (_gameInCourse && _direction != Vector3.zero && _canMove) _rigid.MovePosition(transform.position + transform.TransformDirection(_direction) * _speed * Time.deltaTime);
    }

    //This is for multiplayer only
    public void Initialize()
    {
        hp = 500;
        maxHp = 500;
        mana = 215;
        maxMana = 215;
        hpRegeneration = 2;
        manaRegeneration = 5;
        
        launchParticleSystem = transform.Find("Launch").GetComponent<ParticleSystem>();
        chargeParticleSystem = transform.Find("Charge").GetComponent<ParticleSystem>();
        spellPos = transform.Find("SpellPos");

        launchParticleSystem.transform.position = this.transform.position;
        chargeParticleSystem.transform.position = this.transform.position;
    }

    //Pasado
    #region LockOn
    private Transform GetEnemy()
    {
        var chars = FindObjectsOfType<NetworkCharacter>();
        foreach (var charac in chars)
        {
            if (charac.transform != this.transform)
                return charac.transform;
        }

        return null;
    }

    private bool CheckEnemyDistance()
    {
        if (enemy == null) enemy = GetEnemy();

        var dir = (enemy.position - _cam.position).normalized;
        var ang = Vector3.Angle(_cam.forward, dir);

        return (Vector3.Distance(this.transform.position, enemy.position) > 10f && ang <= 20) ? true : false;
    }
    #endregion

    //Pasado
    #region Inputs
    private void Actions()
    {
        if(!_isDead)
        {

            _direction = new Vector3(InputManager.instance.GetHorizontalMovement(), 0f, InputManager.instance.GetVerticalMovement());
        
        
            //Sets the rotation of the character using the camera
            //if (_direction != Vector3.zero && !_isEvading) transform.forward = new Vector3(_cam.forward.x, 0, _cam.forward.z);
        
            //_canMove = !isAttacking && !_isEvading && !_isCastingSpell;
        
            if (_canMove)
            {
                //set Running
                if (_direction.z != 0) runForward = true;
                else runForward = false;
                if (_direction.x > 0)
                {
                    runRight = true;
                    runLeft = false;
                }
                else if (_direction.x < 0)
                {
                    runRight = false;
                    runLeft = true;
                }
                else
                {
                    runRight = false;
                    runLeft = false;
                }
                if (_direction.z < 0) speedMult = -1.0f;
                else speedMult = 1.0f;

                //Check if running
                if (!InputManager.instance.GetRun() || _direction.z < 0) _speed = _originalWalkSpeed;

                //Do Blocking
                if (InputManager.instance.GetBlocking()) isBlocking = true;
                else isBlocking = false;

                //Do Roll
                if (InputManager.instance.GetDodge()) StartCoroutine(CheckEvade(_dodgeTime));

                //Use Skill
                //if (InputManager.instance.GetEnviromentSkill() && !_isPulling) UseSkill(environmentalSpell, HUDController.Spells.Environmental);
                //else if (InputManager.instance.GetClassSkill()) UseSkill(classSpell, HUDController.Spells.Class);
                //else if (InputManager.instance.GetSkill() && !_isPulling) UseSkill(pickedSpell, HUDController.Spells.Picked);
                //AUNQUE DIGA USESPELL ES UN "WORKAROUND" PARA EL BLINK
                else if (InputManager.instance.GetUseItem() && CheckEnemyDistance())
                {
                    //if (enemy == null) GetEnemy(); //Esto lo dejo comentado para debuggear en caso de que pase algo.
                   // UseSkill(mobilitySpell, HUDController.Spells.Mobility);
                }
            }

            var canAct = _isAttackAvailable && !isEvading && !isCastingSpell && !gtHasObject;

            //Check if can attack, and then checks which attack should do
            if (canAct)
            {
                if (InputManager.instance.GetLightAttack())
                {
                    isAttacking = true;
                    _isAttackAvailable = false;
                    isLightAttack = true;
                    x = true;

                    EventManager.DispatchEvent("AttackEnter", new object[] { _lightAttackDamage });

                    if(!PhotonNetwork.offlineMode) photonView.RPC("SetX", PhotonTargets.All);
                }
                else if (InputManager.instance.GetHeavyAttack())
                {
                    isAttacking = true;
                    _isAttackAvailable = false;
                    isHeavyAttack = true;
                    y = true;

                    EventManager.DispatchEvent("AttackEnter", new object[] { _heavyAttackDamage });

                    if (!PhotonNetwork.offlineMode) photonView.RPC("SetY", PhotonTargets.All);
                }

                if (isAttacking)
                {
                    runForward = false;
                    isBlocking = false;
                }
            }
        }

        Notify();
    }

    #endregion

    //Pasado
    #region Particles
    public GameObject ParticleCaller(GameObject part, Vector3 pos)
    {
        var inst = Instantiate(part, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    [PunRPC]
    public GameObject RpcParticleCaller(string part, Vector3 pos)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }


    public GameObject ParticleCaller(string part, Vector3 pos)
    {
        var parts = Resources.Load(part, typeof(GameObject)) as GameObject;
        var inst = Instantiate(parts, pos, Quaternion.identity);
        Destroy(inst, 3);
        return inst;
    }

    public GameObject ParticleCaller(GameObject part, Transform parent)
    {
        var inst = Instantiate(part, parent);
        Destroy(inst, 3);
        return inst;
    }
    
    public ParticleSystem ParticleDestroyer(string name)
    {
        var p =  spellPos.FindChild(name).GetComponent<ParticleSystem>();
        p.Stop();

        Destroy(p.gameObject, 3f);
        return p;
    }
    #endregion

    //Pasado
    #region TelekineticPhysics
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
            collision.gameObject.GetComponent<TelekineticObject>().ChangeState(PhotonNetwork.player.NickName);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 11)
            collision.gameObject.GetComponent<TelekineticObject>().ChangeState(PhotonNetwork.player.NickName);
    }

    #endregion

    //Pasado
    #region Animation Events
    private void OnIddleEnter()//Pasado
    {
        x = false;
        y = false;
        _isAttackAvailable = true;
        isLightAttack = false;
        isHeavyAttack = false;
        isAttacking = false;

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetBothOff", PhotonTargets.All);
    }
    */

    private void OnSyncTime() //Pasado
    {
        x = false;
        y = false;
        _isAttackAvailable = true;

        if (!PhotonNetwork.offlineMode) photonView.RPC("SetBothOff", PhotonTargets.All);
    }

    /*
    private void OnRollExit()//Pasado
    {
        isEvading = false;
        isRolling = false;
        if (!PhotonNetwork.offlineMode) photonView.RPC("SetRollingOff", PhotonTargets.All);
    }
	
	private void OnDamageExit()//Pasado
    {
        isDamaged = false;
        if (!PhotonNetwork.offlineMode) photonView.RPC("SetDamageOff", PhotonTargets.All);
    }
    #endregion

    #region Unused
    /// <summary>Checks if the camera is too close to the character. If so, sets the material of the character to transparent</summary>
    private void CheckCameraDistance()
    {
        _fixedCharPos = transform.position + new Vector3(0f, 1.4f, 0f);

        if (_mr == null) _mr = GetComponentInChildren<SkinnedMeshRenderer>();

        if (Vector3.Distance(_fixedCharPos, transform.TransformPoint(_camObj.transform.localPosition)) <= 1f)
            _mr.material = transparentMat;
        else
            _mr.material = defaultMat;
    }
    #endregion

    private void OnGameFinished(params object[] paramsContainer)
    {
        _gameInCourse = false;
    }

    //Pasado
    #region Skills
    /// <summary>Decides if it needs to wait or not to launch an spell</summary>
    private void UseSkill(ISpell spell, HUDController.Spells pickType)
    {
        var canCast = !spell.IsInCooldown() && spell.GetManaCost() < Mana;
        
        if (spell.GetCastType() == CastType.INSTANT)
        {
            if (canCast)
            {
                spell.UseSpell();
                EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType });
                StartCoroutine(SpellCooldown(spell, pickType));
            }
        }
        else if (spell.GetCastType() == CastType.DELAYED)
        {
            if (canCast)
            {
                StartCoroutine(CastSpell(spell.CastTime(), spell, pickType));
            }
        }
    }

    void OnObjectPulling(params object[] paramsContainer)
    {
        _isPulling = true;
    }

    void OnObjectPulled(params object[] paramsContainer)
    {
        _isPulling = false;
    }

    #endregion

    //Pasado
    #region StatsUpdate
    void OnSpellCasted(params object[] paramsContainer)
    {
        ConsumeMana((int)paramsContainer[0]);
    }

    void ConsumeMana(float cost)
    {
        Mana -= cost;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("ManaUpdate", new object[] { Mana, fill });
    }

    void RegainMana(float regained)
    {
        Mana += regained;
        float fill = Mana / maxMana;
        EventManager.DispatchEvent("ManaUpdate", new object[] { Mana, fill });
    }
    void LoseHP(float damage)
    {
        var dmg = isBlocking ? damage / 3 : damage;
        float fill = (Hp - dmg) / maxHp;
        if (fill < 0) fill = 0;
        EventManager.DispatchEvent("LifeUpdate", new object[] { (dmg > Hp ? 0 : Hp-dmg), fill });
        Hp = dmg >= Hp ? 0 : Hp - dmg;
    }
    void RegainHp(float regained)
    {
        if (_gameInCourse)
        {
            Hp += regained;
            float fill = Hp / maxHp;
            EventManager.DispatchEvent("LifeUpdate", new object[] { Hp, fill });
        }
    }
    
    public void TakeDamage(float damage)
    {
        EventManager.DispatchEvent("CharacterDamaged", new object[] { transform.position, this });
        LoseHP(damage);
    }

    #endregion

    //Obsoleto
    #region ObserverMethods
    /// <summary>Adds an observer to the list of observing elements</summary>
    public void Subscribe(IObserver observer)
    {
        _observers.Add(observer);
    }

    /// <summary>Removes an observer from the list of observing elements</summary>
    public void Unsubscribe(IObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <summary>Notifies to all the elements on the list of observing elements</summary>
    private void Notify()
    {
        foreach (var obs in _observers)
        {
            obs.OnNotify();
        }
    }
    #endregion

    //Pasado
    #region Corutines
    /// <summary>Waits a certain time to launch the spell</summary>
    IEnumerator CastSpell(float time, ISpell spell, HUDController.Spells pickType)
    {
        var vTemp = new Vector3(transform.position.x, transform.position.y + 0.66f, transform.position.z);
        EventManager.DispatchEvent("RepulsiveTelekinesisLoad", new object[] { vTemp, this });

        isCastingSpell = true;

        var w = new WaitForSeconds(time);
        yield return w;

        pickedSpell.UseSpell();
        isCastingSpell = false;

       
        EventManager.DispatchEvent("SpellCooldown", new object[] { spell.CooldownTime(), pickType });
        StartCoroutine(SpellCooldown(spell, pickType));
    }

    /// <summary>Checks if the character is going to Run or going to Roll</summary>
    IEnumerator CheckEvade(float time)
    {
        var w = new WaitForSeconds(time);
        yield return w;

        if (!InputManager.instance.GetRun() && !isEvading && !gtHasObject)
        {
            isEvading = true;
            if(_direction != Vector3.zero) transform.forward = new Vector3(transform.TransformDirection(_direction).x, 0f, transform.TransformDirection(_direction).z);
            isRolling = true;
            if (!PhotonNetwork.offlineMode) photonView.RPC("SetRollingOn", PhotonTargets.All);
        }
        else if(!isBlocking) _speed = _originalRunSpeed;
    }

    IEnumerator SpellCooldown(ISpell spell, HUDController.Spells pickType)
    {
        spell.EnterInCooldown();
        yield return new WaitForSeconds(spell.CooldownTime());
        spell.ExitFromCooldown();
    }

    IEnumerator Regeneration()
    {
        float tick = 0.16f;
        float hpRegenByTick = hpRegeneration * tick;
        float manaRegenByTick = manaRegeneration * tick;
        
        while (true)
        {
            RegainHp(hpRegenByTick);
            RegainMana(manaRegenByTick);
            yield return new WaitForSeconds(tick);
        }
    }
    #endregion
    */
}
