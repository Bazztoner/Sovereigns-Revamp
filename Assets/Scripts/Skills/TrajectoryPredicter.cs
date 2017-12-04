using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredicter : MonoBehaviour
{
    // Reference to the LineRenderer we will use to display the simulated path
    public LineRenderer ln;

    TelekineticObject target;

    // Number of segments to calculate - more gives a smoother line
    public int segmentCount = 20;
    int _actualSegCount;

    // Length scale for each segment
    public float segmentScale = 1;

    // gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
    private Collider _hitObject;
    public Collider hitObject { get { return _hitObject; } }

    public bool _trailActivated;

    public void Init(TelekineticObject tgt)
    {
        EventManager.AddEventListener("TelekinesisObjectPulled", OnObjectPulled);
        EventManager.AddEventListener("TelekinesisObjectLaunched", OnObjectLaunched);

        ln = GetComponent<LineRenderer>();

        ln.enabled = false;

        target = tgt;
    }

    void OnObjectPulled(object[] paramsContainer)
    {
        var tgt = (TelekineticObject)paramsContainer[2];

        int mask = default(int);
        if (GameManager.screenDivided)
        {
            var trn = (Transform)paramsContainer[0];
            mask = trn.GetComponentInParent<PlayerInput>().gameObject.name == "Player1" ?
                       Utilities.IntLayers.VISIBLETOP1 : Utilities.IntLayers.VISIBLETOP2;
        }
       
        if (tgt == target)
        {
            if (GameManager.screenDivided) gameObject.layer = mask;
             _trailActivated = true;
            ActivateRendering(_trailActivated);
        }
    }

    void OnObjectLaunched(object[] paramsContainer)
    {
        var tgt = (TelekineticObject)paramsContainer[2];
        if (tgt == target)
        {
            if (GameManager.screenDivided) gameObject.layer = Utilities.IntLayers.VISIBLETOBOTH;
            _trailActivated = false;
            ActivateRendering(_trailActivated);
        }
    }

    public void ActivateRendering(bool activate)
    {
        ln.enabled = activate;
    }

    void FixedUpdate()
    {
        if (ln.enabled) SimulatePath();
    }

    /// <summary>
    /// Simulate the path of a launched ball.
    /// Slight errors are inherent in the numerical method used.
    /// </summary>
    void SimulatePath()
    {
        if (target.Camera == null) return;

        Vector3[] segments = new Vector3[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        segments[0] = transform.position;

        if (target == null)
        {
            target = GetComponentInParent<TelekineticObject>();
        }

        var rb = target.GetComponent<Rigidbody>();
        // The initial velocity
        Vector3 segVelocity = target.GetLaunchDirection()
                            * target.throwForce
                            * rb.mass
                            * (1-rb.drag)
                            * Time.fixedDeltaTime;

        // reset our hit object
        _hitObject = null;

        bool hasCollision = false;

        for (int i = 1; i < segmentCount; i++)
        {
            if (hasCollision)
            {
                _actualSegCount = i;
                break;
            }
            // Time it takes to traverse one segment of length segScale (careful if velocity is zero)
            float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;

            // Add velocity from gravity for this segment's timestep
            segVelocity = segVelocity + Physics.gravity * segTime;

            // Check to see if we're going to hit a physics object
            RaycastHit hit;
            if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale))
            {
                _hitObject = hit.collider;
                if (!_hitObject.isTrigger)
                {
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                    hasCollision = true;
                }
                else segments[i] = segments[i - 1] + segVelocity * segTime;
            }
            // If our raycast hit no objects, then set the next position to the last one plus v*t
            else segments[i] = segments[i - 1] + segVelocity * segTime;
        }

        if (!hasCollision) _actualSegCount = segmentCount;
        // At the end, apply our simulations to the LineRenderer

        ln.positionCount = _actualSegCount;

        for (int i = 0; i < _actualSegCount; i++)
        {
            ln.SetPosition(i, segments[i]);

        }

    }
}
