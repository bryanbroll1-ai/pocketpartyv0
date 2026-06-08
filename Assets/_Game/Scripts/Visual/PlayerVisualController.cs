using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private Color bodyColor = PartyArtPalette.PlayerBlue;
    [SerializeField] private PlayerAvatarMood mood = PlayerAvatarMood.Idle;
    [SerializeField] private bool animate = true;

    private Transform body;
    private Transform head;
    private Transform leftArm;
    private Transform rightArm;
    private Transform leftFoot;
    private Transform rightFoot;
    private Vector3 startPosition;
    private bool built;

    public void Configure(int index, Color color)
    {
        playerIndex = index;
        bodyColor = color;
        if (built)
        {
            ApplyMaterials();
        }
    }

    public void SetMood(PlayerAvatarMood newMood)
    {
        mood = newMood;
    }

    private void Awake()
    {
        BuildIfNeeded();
        startPosition = transform.localPosition;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && bodyColor.a <= 0f)
        {
            bodyColor = PartyArtPalette.PlayerColor(playerIndex);
        }
    }

    private void Update()
    {
        if (!animate || !built)
        {
            return;
        }

        float time = Time.time + playerIndex * 0.47f;
        float bob = Mathf.Sin(time * GetMoodSpeed()) * GetMoodAmplitude();
        transform.localPosition = startPosition + Vector3.up * bob;

        float armSwing = Mathf.Sin(time * GetMoodSpeed() * 1.25f) * GetArmSwing();
        leftArm.localRotation = Quaternion.Euler(0f, 0f, 18f + armSwing);
        rightArm.localRotation = Quaternion.Euler(0f, 0f, -18f - armSwing);
        leftFoot.localRotation = Quaternion.Euler(armSwing * 0.35f, 0f, 0f);
        rightFoot.localRotation = Quaternion.Euler(-armSwing * 0.35f, 0f, 0f);

        float squash = mood == PlayerAvatarMood.Jump ? 1f + Mathf.Abs(Mathf.Sin(time * 6f)) * 0.08f : 1f;
        body.localScale = new Vector3(0.42f / squash, 0.54f * squash, 0.36f / squash);
        head.localScale = Vector3.one * (0.54f + bob * 0.035f);
    }

    private void BuildIfNeeded()
    {
        if (built)
        {
            return;
        }

        ClearChildren();
        body = CreatePrimitive("Rounded Body", PrimitiveType.Capsule, new Vector3(0f, 0.68f, 0f), new Vector3(0.42f, 0.54f, 0.36f), PartyArtMaterials.CreateTinted("Avatar Body", bodyColor)).transform;
        head = CreatePrimitive("Big Friendly Head", PrimitiveType.Sphere, new Vector3(0f, 1.25f, 0f), Vector3.one * 0.54f, PartyArtMaterials.Get(PartyArtMaterialType.Skin)).transform;
        leftArm = CreatePrimitive("Left Arm", PrimitiveType.Capsule, new Vector3(-0.38f, 0.78f, 0f), new Vector3(0.13f, 0.32f, 0.13f), PartyArtMaterials.CreateTinted("Avatar Arm", bodyColor * 0.92f)).transform;
        rightArm = CreatePrimitive("Right Arm", PrimitiveType.Capsule, new Vector3(0.38f, 0.78f, 0f), new Vector3(0.13f, 0.32f, 0.13f), PartyArtMaterials.CreateTinted("Avatar Arm", bodyColor * 0.92f)).transform;
        CreatePrimitive("Chest Badge", PrimitiveType.Sphere, new Vector3(0f, 0.82f, -0.35f), new Vector3(0.13f, 0.09f, 0.04f), PartyArtMaterials.Get(PartyArtMaterialType.WhiteAccent));
        CreatePrimitive("Color Cap", PrimitiveType.Sphere, new Vector3(0f, 1.55f, -0.02f), new Vector3(0.34f, 0.14f, 0.30f), PartyArtMaterials.CreateTinted("Avatar Color Cap", bodyColor));
        leftFoot = CreatePrimitive("Left Foot", PrimitiveType.Sphere, new Vector3(-0.16f, 0.14f, 0.09f), new Vector3(0.20f, 0.12f, 0.30f), PartyArtMaterials.Get(PartyArtMaterialType.DarkAccent)).transform;
        rightFoot = CreatePrimitive("Right Foot", PrimitiveType.Sphere, new Vector3(0.16f, 0.14f, 0.09f), new Vector3(0.20f, 0.12f, 0.30f), PartyArtMaterials.Get(PartyArtMaterialType.DarkAccent)).transform;
        CreatePrimitive("Face Left Eye", PrimitiveType.Sphere, new Vector3(-0.13f, 1.31f, -0.46f), Vector3.one * 0.055f, PartyArtMaterials.Get(PartyArtMaterialType.DarkAccent));
        CreatePrimitive("Face Right Eye", PrimitiveType.Sphere, new Vector3(0.13f, 1.31f, -0.46f), Vector3.one * 0.055f, PartyArtMaterials.Get(PartyArtMaterialType.DarkAccent));
        CreatePrimitive("Soft Smile", PrimitiveType.Cube, new Vector3(0f, 1.18f, -0.47f), new Vector3(0.20f, 0.026f, 0.026f), PartyArtMaterials.Get(PartyArtMaterialType.DarkAccent));
        CreatePrimitive("Ground Contact Pad", PrimitiveType.Cylinder, new Vector3(0f, 0.025f, 0f), new Vector3(0.58f, 0.018f, 0.42f), PartyArtMaterials.CreateTinted("Avatar Contact Pad", new Color(0.58f, 0.66f, 0.70f, 1f)));

        built = true;
        ApplyMaterials();
    }

    private GameObject CreatePrimitive(string objectName, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject instance = GameObject.CreatePrimitive(primitiveType);
        instance.name = objectName;
        instance.transform.SetParent(transform, false);
        instance.transform.localPosition = localPosition;
        instance.transform.localScale = localScale;
        instance.GetComponent<Renderer>().material = material;

        Collider collider = instance.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        return instance;
    }

    private void ApplyMaterials()
    {
        if (body == null)
        {
            return;
        }

        body.GetComponent<Renderer>().material = PartyArtMaterials.CreateTinted("Avatar Body", bodyColor);
        leftArm.GetComponent<Renderer>().material = PartyArtMaterials.CreateTinted("Avatar Arm", bodyColor * 0.92f);
        rightArm.GetComponent<Renderer>().material = PartyArtMaterials.CreateTinted("Avatar Arm", bodyColor * 0.92f);
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private float GetMoodSpeed()
    {
        switch (mood)
        {
            case PlayerAvatarMood.Run:
                return 8f;
            case PlayerAvatarMood.Jump:
                return 6f;
            case PlayerAvatarMood.Hit:
                return 10f;
            case PlayerAvatarMood.Win:
                return 7f;
            case PlayerAvatarMood.Lose:
                return 3f;
            default:
                return 3.2f;
        }
    }

    private float GetMoodAmplitude()
    {
        switch (mood)
        {
            case PlayerAvatarMood.Run:
                return 0.06f;
            case PlayerAvatarMood.Jump:
                return 0.14f;
            case PlayerAvatarMood.Hit:
                return 0.03f;
            case PlayerAvatarMood.Win:
                return 0.10f;
            case PlayerAvatarMood.Lose:
                return 0.02f;
            default:
                return 0.035f;
        }
    }

    private float GetArmSwing()
    {
        switch (mood)
        {
            case PlayerAvatarMood.Run:
                return 38f;
            case PlayerAvatarMood.Win:
                return 56f;
            case PlayerAvatarMood.Lose:
                return 8f;
            default:
                return 18f;
        }
    }
}
