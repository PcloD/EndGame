using Mirror;
using UnityEngine;

public class SmoothToZero : NetworkBehaviour
{
    public Transform SmoothTarget;

    /// <summary>
    /// How quickly to smooth to zero.
    /// </summary>
    [Tooltip("How quickly to smooth to zero.")] [SerializeField]
    private float _smoothRate = 20f;

    /// <summary>
    /// True if subscribed to FixedUpdateManager events.
    /// </summary>
    private bool _subscribed = false;

    /// <summary>
    /// Position before simulation is performed.
    /// </summary>
    private Vector3 _position;

    /// <summary>
    /// Rotation before simulation is performed.
    /// </summary>
    private Quaternion _rotation;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SubscribeToFixedUpdateManager(true);
    }

    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        SubscribeToFixedUpdateManager(false);
    }

    private void OnEnable()
    {
        if (base.hasAuthority) SubscribeToFixedUpdateManager(true);
    }

    private void OnDisable()
    {
        SubscribeToFixedUpdateManager(false);
    }

    private void Update()
    {
        if (base.hasAuthority)
        {
            Smooth();
        }
    }

    /// <summary>
    /// Smooths position and rotation to zero values.
    /// </summary>
    private void Smooth()
    {
        float distance;
        distance = Mathf.Max(0.01f, Vector3.Distance(SmoothTarget.localPosition, Vector3.zero));
        SmoothTarget.localPosition = Vector3.MoveTowards(SmoothTarget.localPosition, Vector3.zero,
            distance * _smoothRate * Time.deltaTime);
        distance = Mathf.Max(1f, Quaternion.Angle(SmoothTarget.localRotation, Quaternion.identity));
        SmoothTarget.localRotation = Quaternion.RotateTowards(SmoothTarget.localRotation, Quaternion.identity,
            distance * _smoothRate * Time.deltaTime);
    }

    /// <summary>
    /// Changes event subscriptions on the FixedUpdateManager.
    /// </summary>
    /// <param name="subscribe"></param>
    private void SubscribeToFixedUpdateManager(bool subscribe)
    {
        if (subscribe == _subscribed) return;

        if (subscribe)
        {
            FixedUpdateManager.OnPreFixedUpdate += FixedUpdateManager_OnPreFixedUpdate;
            FixedUpdateManager.OnPostFixedUpdate += FixedUpdateManager_OnPostFixedUpdate;
        }
        else
        {
            FixedUpdateManager.OnPreFixedUpdate -= FixedUpdateManager_OnPreFixedUpdate;
            FixedUpdateManager.OnPostFixedUpdate -= FixedUpdateManager_OnPostFixedUpdate;
        }

        _subscribed = subscribe;
    }

    private void FixedUpdateManager_OnPostFixedUpdate()
    {
        SmoothTarget.position = _position;
        SmoothTarget.rotation = _rotation;
    }

    private void FixedUpdateManager_OnPreFixedUpdate()
    {
        _position = SmoothTarget.position;
        _rotation = SmoothTarget.rotation;
    }
}