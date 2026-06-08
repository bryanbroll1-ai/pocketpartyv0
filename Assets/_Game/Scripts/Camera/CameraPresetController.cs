using UnityEngine;

public class CameraPresetController : MonoBehaviour
{
    [SerializeField] private PartyCameraPreset currentPreset = PartyCameraPreset.Isometric;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 focusPoint = Vector3.zero;

    private Camera controlledCamera;

    private void Awake()
    {
        controlledCamera = GetComponent<Camera>();
        ApplyPreset(currentPreset);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || TappedTopRight())
        {
            NextPreset();
        }

        if (currentPreset == PartyCameraPreset.ThirdPersonFollow && followTarget != null)
        {
            Vector3 desired = followTarget.position + new Vector3(0f, 3.0f, -5.0f);
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * 4f);
            LookAt(followTarget.position + Vector3.up * 0.7f);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void NextPreset()
    {
        int next = ((int)currentPreset + 1) % System.Enum.GetValues(typeof(PartyCameraPreset)).Length;
        ApplyPreset((PartyCameraPreset)next);
    }

    public void ApplyPreset(PartyCameraPreset preset)
    {
        currentPreset = preset;
        if (controlledCamera == null)
        {
            controlledCamera = GetComponent<Camera>();
        }

        if (controlledCamera != null)
        {
            controlledCamera.fieldOfView = preset == PartyCameraPreset.TopDownArena ? 42f : 48f;
            controlledCamera.backgroundColor = PartyArtPalette.Sky;
        }

        switch (preset)
        {
            case PartyCameraPreset.FixedFront:
                transform.position = new Vector3(0f, 5.4f, -13.0f);
                LookAt(focusPoint + Vector3.up * 0.8f);
                break;
            case PartyCameraPreset.SideView:
                transform.position = new Vector3(13.0f, 5.2f, -1.2f);
                LookAt(focusPoint + Vector3.up * 0.7f);
                break;
            case PartyCameraPreset.TopDownArena:
                transform.position = new Vector3(0f, 14.0f, -0.2f);
                transform.rotation = Quaternion.Euler(88f, 0f, 0f);
                break;
            case PartyCameraPreset.ThirdPersonFollow:
                if (followTarget != null)
                {
                    transform.position = followTarget.position + new Vector3(0f, 3.6f, -6.2f);
                    LookAt(followTarget.position + Vector3.up * 0.85f);
                }
                else
                {
                    transform.position = new Vector3(0f, 4.0f, -8.0f);
                    LookAt(focusPoint);
                }

                break;
            default:
                transform.position = new Vector3(0f, 10.2f, -13.2f);
                LookAt(focusPoint + new Vector3(0f, 0.55f, 0.25f));
                break;
        }
    }

    private void LookAt(Vector3 target)
    {
        transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
    }

    private bool TappedTopRight()
    {
        if (Input.touchCount == 0)
        {
            return false;
        }

        Touch touch = Input.GetTouch(0);
        return touch.phase == TouchPhase.Began && touch.position.x > Screen.width * 0.72f && touch.position.y > Screen.height * 0.72f;
    }
}
