using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Esta clase es para cada objeto pequeño que sea interactuable por Telekinesis
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class TelekineticObject : Photon.MonoBehaviour
{
    public float damage;
    public float throwForce;
    public int life; //Vida en enteros. Cada golpe contra objetos (Excepto algunos específicos TODO) quita 1.
    public float pullSpeed;
    public static List<TelekineticObject> allObjs;
    public TrajectoryPredicter predicter;
    public string particleName;

    bool _isLaunched;
    bool _isPulled;
    bool _isGrabbed;
    bool _hasToSync;
    bool _isReadyToDestroy;

    int _enemyLayer = 8;
    int _playerLayer = 13;

    Vector3 _prevPos;
    Quaternion _prevRot;

    LayerMask _mask;

    Camera _cam;
    public Camera Camera
    {
        get { return _cam; }
    }
    public Vector3 correctionVector;

    RaycastHit _hit;

    Rigidbody _rigid;

    Transform _pullPos;

    public bool IsGrabbed
    {
        get { return _isGrabbed; }
    }

    public bool IsLaunched
    {
        get { return _isLaunched; }
    }

    public bool IsPulled
    {
        get { return _isPulled; }
    }

    public bool IsReadyToDestroy
    {
        get { return _isReadyToDestroy; }
    }

    void Awake()
    {
        if (allObjs == null) allObjs = new List<TelekineticObject>();
        allObjs.Add(this);

        _prevPos = this.transform.position;
        _prevRot = this.transform.rotation;
        _hasToSync = false;

        _rigid = GetComponent<Rigidbody>();
        _rigid.isKinematic = true;

        _mask = PhotonNetwork.offlineMode ? 1 << LayerMask.NameToLayer("Enemy") : 1 << LayerMask.NameToLayer("Player");

        if (!GameManager.screenDivided)
        {
            _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }

    void Start()
    {
        EventManager.AddEventListener("DividedScreen", OnDividedScreen);

        var tp = GameObject.Instantiate(Resources.Load("Spells/Dummies/TrayectoryDummy") as GameObject);
        tp.transform.SetParent(transform);
        tp.transform.localPosition = Vector3.zero;
        predicter = tp.GetComponent<TrajectoryPredicter>();
        predicter.Init(this);

        correctionVector = new Vector3(0f, .3f, 0f);
    }

    void OnEnable()
    {
        if (GameManager.screenDivided) EventManager.AddEventListener("TelekinesisObjectPulled", OnObjectPulled);
    }

    void OnDividedScreen(object[] paramsContainer)
    {
        EventManager.AddEventListener("TelekinesisObjectPulled", OnObjectPulled);
    }

    void OnObjectPulled(object[] paramsContainer)
    {
        var tgt = (TelekineticObject)paramsContainer[2];
        if (tgt.gameObject == gameObject)
        {
            print("OnObjectPulled " + gameObject.name);
            var trn = (Transform)paramsContainer[0];
            var cam = trn.GetComponentInParent<Player1Input>().GetCamera;
            _cam = cam.GetCamera;
        }
    }

    void TakeDamage()
    {
        life -= 1;
        if (life <= 0) StartCoroutine(DestroyTelekineticObject(3f));
    }
    IEnumerator DestroyTelekineticObject(float time)
    {
        EventManager.RemoveEventListener("TelekinesisObjectPulled", OnObjectPulled);
        if (particleName == null || particleName == default(string))
        {
            particleName = "JarParticle";
        }
        var part = GameObject.Instantiate(Resources.Load("Particles/" + particleName) as GameObject, transform.position, Quaternion.identity);
        Destroy(part, time);
        GetComponent<Renderer>().enabled = false;
        _isReadyToDestroy = true;
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        allObjs.Remove(this);
    }

    public void MoveToObjective()
    {
        if (_isPulled && _pullPos != null)
        {
            transform.SetParent(_pullPos.parent);

            var direction = (_pullPos.localPosition - transform.localPosition).normalized;
            
            transform.localPosition += direction * pullSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.localPosition, _pullPos.localPosition) <= 0.5)
            {
                transform.localPosition = _pullPos.localPosition;
                _isPulled = false;
            } 
        }
    }

    void Update()
    {
        if (!PhotonNetwork.offlineMode && PhotonNetwork.inRoom && _hasToSync && (this.transform.position != _prevPos || this.transform.rotation != _prevRot))
            photonView.RPC("SyncObj", PhotonTargets.All, this.transform.position, this.transform.rotation, PhotonNetwork.player.NickName);
    }

    void FixedUpdate()
    {
        MoveToObjective();
    }

    public void PullObject(Transform to)
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        
        _rigid.constraints = RigidbodyConstraints.FreezeAll;
        _rigid.isKinematic = true;
        _rigid.useGravity = false;

        _isPulled = true;
        _hasToSync = true;
        _pullPos = to;
    }

    public void ActivateFromDestruction()
    {
        _isLaunched = true;
        _isPulled = false;
    }

    public void DeactivateFromDestruction()
    {
        _isLaunched = false;
        _isPulled = false;
    }

    public void SetGrabbed(bool grabbed)
    {
        _isGrabbed = grabbed;
    }

    public void LaunchObject()
    {
        _isLaunched = true;

        transform.SetParent(GameObject.Find("TelekinesisObjects").transform);

        ActivateAndLaunchObject();

        _isPulled = false;
    }

    public void RepelObject()
    {
        _isLaunched = true;

        ActivateObject();

        _isPulled = false;
    }

    public void ActivateObject()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = false;
        
        _rigid.constraints = RigidbodyConstraints.None;
        _rigid.isKinematic = false;
        _rigid.useGravity = true;

        _isPulled = false;

        StartCoroutine(ChangeKinematics(3f));
    }

    public void ActivateAndLaunchObject()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = false;
        
        _rigid.constraints = RigidbodyConstraints.None;
        _rigid.isKinematic = false;
        _hasToSync = true;
        
        Vector3 launchDirection = GetLaunchDirection();
        
        _rigid.AddForce(launchDirection * throwForce);
        _rigid.useGravity = true;

        EventManager.DispatchEvent("ObjectPulled");
        _isPulled = false;

        StartCoroutine(ChangeKinematics(3f));
    }

    public Vector3 GetLaunchDirection()
    {
        Vector3 targetPoint = Vector3.zero;

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out _hit, 200f))
            targetPoint = _hit.point;

        return targetPoint != Vector3.zero ? (targetPoint - transform.parent.TransformPoint(transform.localPosition)).normalized : transform.parent.forward;
    }

    [PunRPC]
    public void SyncObj(Vector3 pos, Quaternion rot, string user)
    {
        if (PhotonNetwork.player.NickName != user)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, pos, 0.1f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, 0.1f);

            _prevPos = this.transform.position;
            _prevRot = this.transform.rotation;
        }
    }

    public void ChangeState(string user)
    {
        if (PhotonNetwork.player.NickName == user)
        {
            if (_rigid.isKinematic)
            {
                _rigid.isKinematic = false;
                _hasToSync = true;
            } 
            else StartCoroutine(ChangeKinematics(1.5f));
        }
    }

    IEnumerator ChangeKinematics(float time)
    {
        yield return new WaitForSeconds(time);
        _rigid.isKinematic = true;
        _hasToSync = false;
    }

    public void DeactivateObject()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        
        _rigid.constraints = RigidbodyConstraints.FreezeAll;
        _rigid.isKinematic = true;
        _rigid.useGravity = false;
    }
    
    void OnCollisionEnter(Collision c)
    {
        if (_isLaunched)
        {
            //TODO agregar distintos objetos para quitar distinta cantidad de vida
            //TODO evento para hacer daño a entity

            if (PhotonNetwork.offlineMode && c.gameObject.layer == _enemyLayer)
                c.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage, "Telekinetic Object");
            else if (!PhotonNetwork.offlineMode && c.gameObject.layer == _playerLayer)
                c.gameObject.GetComponentInParent<DataSync>().photonView.RPC("TakeDamage", PhotonTargets.All, damage, PhotonNetwork.player.NickName, "Telekinetic Object");
            
            TakeDamage();
            _isLaunched = false;
        }
    }
}
