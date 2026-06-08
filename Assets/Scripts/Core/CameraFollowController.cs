using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 6.8f, -7.2f);
    [SerializeField] private float followSmoothTime = 0.22f;
    [SerializeField] private float lookAhead = 0.75f;

    private Transform target;
    private Vector3 velocity;
    private Vector3 overviewTarget = new Vector3(0f, 0f, 0f);

    private void Awake()
    {
        var camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.orthographic = false;
            camera.fieldOfView = 42f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ShowBoardOverview()
    {
        target = null;
        SnapToDesiredPosition();
    }

    public void SnapToDesiredPosition()
    {
        transform.position = GetDesiredPosition();
        LookAtFocus();
        velocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = GetDesiredPosition();
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSmoothTime);
        LookAtFocus();
    }

    private Vector3 GetFocus()
    {
        return target != null ? target.position + Vector3.up * lookAhead : overviewTarget;
    }

    private Vector3 GetDesiredPosition()
    {
        return GetFocus() + followOffset;
    }

    private void LookAtFocus()
    {
        Vector3 focus = GetFocus();
        transform.rotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
    }
}
