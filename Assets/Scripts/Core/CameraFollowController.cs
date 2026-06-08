using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 7.0f, -7.4f);
    [SerializeField] private float followSmoothTime = 0.22f;
    [SerializeField] private float lookAhead = 0.75f;

    private Transform target;
    private Vector3 velocity;
    private Vector3 overviewTarget = new Vector3(0f, 0f, 0f);
    private Vector3 smoothedBase;
    private float shakeAmount;
    private bool initialized;

    // Called by gameplay moments (dice roll, coin gain, win) for a quick kick.
    public void AddShake(float amount)
    {
        shakeAmount = Mathf.Min(shakeAmount + amount, 0.6f);
    }

    private void Awake()
    {
        var camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.orthographic = false;
            camera.fieldOfView = 38f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 140f;
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
        smoothedBase = GetDesiredPosition();
        transform.position = smoothedBase;
        LookAtFocus();
        velocity = Vector3.zero;
        initialized = true;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = GetDesiredPosition();
        if (!initialized)
        {
            smoothedBase = desiredPosition;
            initialized = true;
        }

        smoothedBase = Vector3.SmoothDamp(smoothedBase, desiredPosition, ref velocity, followSmoothTime);

        shakeAmount = Mathf.MoveTowards(shakeAmount, 0f, Time.deltaTime * 1.6f);
        Vector3 sway = new Vector3(
            Mathf.Sin(Time.time * 0.55f) * 0.045f,
            Mathf.Sin(Time.time * 0.42f + 1.3f) * 0.03f,
            0f);
        Vector3 shake = shakeAmount > 0f
            ? new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * shakeAmount
            : Vector3.zero;

        transform.position = smoothedBase + sway + shake;
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
