using UnityEngine;

// Gentle idle motion for world props (water patches, crystals, etc.).
public class BobMotion : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.04f;
    [SerializeField] private float bobSpeed = 1.5f;
    [SerializeField] private float spinSpeed = 0f;

    private Vector3 basePosition;
    private float phase;

    public void Configure(float height, float speed, float spin)
    {
        bobHeight = height;
        bobSpeed = speed;
        spinSpeed = spin;
    }

    private void Start()
    {
        basePosition = transform.localPosition;
        phase = Random.value * 6.28f;
    }

    private void Update()
    {
        if (bobHeight > 0f)
        {
            Vector3 p = basePosition;
            p.y += Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight;
            transform.localPosition = p;
        }

        if (spinSpeed != 0f)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }
    }
}
