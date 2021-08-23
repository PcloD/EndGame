using Assets.Scripts.Utils;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Transform TrackedTransform;
    /// <summary>
    /// The position of this transform is network Synced via a component on the Player Transform (FlexNetworkTransform)
    /// </summary>
    public static Transform TrackedIKTransform;


    public static RaycastHit RayMouseHit;

    [SerializeField]private LayerMask layerMask;
    
    public Transform CameraTransform;
    public Vector3 CameraOffset;
    [HideInInspector] public Vector3 MousePosition;
    public float MovementSpeed;
    public float RotationSpeed;

    private Camera camera;
    [SerializeField] private MinimapController minimapController;

    public static CameraManager Instance;

    void Awake()
    {
        Instance = this;
        CameraTransform = transform;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (TrackedTransform == null) return;

        var rot = Quaternion.LookRotation(TrackedTransform.position - CameraTransform.position);
        var rotToEuler = rot.eulerAngles;
        rotToEuler.y = 0f;
        CameraTransform.eulerAngles = Vector3.Lerp(CameraTransform.eulerAngles, rotToEuler, Time.fixedDeltaTime * RotationSpeed);
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, TrackedTransform.position + CameraOffset, Time.fixedDeltaTime * MovementSpeed);

        if (TrackedIKTransform == null) return;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray,out RayMouseHit,50f,layerMask))
        {
            // ground layer
            if (RayMouseHit.transform.gameObject.layer == 8)
            {
                MouseCursorManager.Instance.CurrentCursorType = MouseCursorManager.CursorType.Default;
            }

            if (RayMouseHit.transform.gameObject.layer == 7)
            {
                if (RayMouseHit.transform == TrackedTransform)
                {
                    MouseCursorManager.Instance.CurrentCursorType = MouseCursorManager.CursorType.DefaultCharacter;
                    return;
                }
                MouseCursorManager.Instance.CurrentCursorType = MouseCursorManager.CursorType.Attack;
                MouseCursorManager.Instance.EnemyEntity = RayMouseHit.transform.gameObject;
            }
        }
    }

    public void SetMinimapTarget(Transform target){
        minimapController.SetTarget(target);
    }
}
