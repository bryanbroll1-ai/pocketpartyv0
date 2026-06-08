using UnityEngine;

public class RollingNumberDisplay : MonoBehaviour
{
    private TextMesh numberText;
    private TextMesh statusText;
    private int currentNumber = 1;
    private bool stopped;
    private float rotationTimer;

    public int CurrentNumber => currentNumber;
    public bool IsStopped => stopped;

    public void Build(Color playerColor)
    {
        ClearChildren();
        AddCube("Number Back Plate", new Vector3(0f, 0f, 0f), new Vector3(0.92f, 0.10f, 0.62f), PartyArtMaterialType.WhiteAccent);
        AddCube("Number Color Strip", new Vector3(0f, 0.08f, -0.34f), new Vector3(0.92f, 0.08f, 0.10f), PartyArtMaterials.CreateTinted("Lucky Roll Strip", playerColor));
        numberText = CreateText("Number", new Vector3(0f, 0.14f, -0.40f), 96, 0.027f, PartyArtPalette.DarkAccent);
        statusText = CreateText("Status", new Vector3(0f, 0.12f, 0.32f), 34, 0.025f, playerColor);
        ResetRoll();
    }

    public void ResetRoll()
    {
        stopped = false;
        SetStatus("ROLL");
        SetNumber(1);
    }

    public void TickRolling(float deltaTime, float speed)
    {
        if (stopped)
        {
            return;
        }

        rotationTimer += deltaTime * speed;
        if (rotationTimer >= 1f)
        {
            int steps = Mathf.FloorToInt(rotationTimer);
            rotationTimer -= steps;
            SetNumber(((currentNumber + steps - 1) % 6) + 1);
        }
    }

    public int Stop()
    {
        stopped = true;
        SetStatus("LOCK");
        return currentNumber;
    }

    public void ShowFinal(int number)
    {
        stopped = true;
        SetNumber(number);
        SetStatus("FINAL");
    }

    public void ShowTieBreaker()
    {
        stopped = false;
        SetStatus("TIE");
    }

    private void SetNumber(int number)
    {
        currentNumber = Mathf.Clamp(number, 1, 6);
        if (numberText != null)
        {
            numberText.text = currentNumber.ToString();
        }
    }

    private void SetStatus(string value)
    {
        if (statusText != null)
        {
            statusText.text = value;
        }
    }

    private TextMesh CreateText(string name, Vector3 localPosition, int fontSize, float characterSize, Color color)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localRotation = Quaternion.Euler(64f, 0f, 0f);
        TextMesh mesh = textObject.AddComponent<TextMesh>();
        mesh.fontSize = fontSize;
        mesh.characterSize = characterSize;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = color;
        return mesh;
    }

    private GameObject AddCube(string name, Vector3 localPosition, Vector3 localScale, PartyArtMaterialType materialType)
    {
        return AddCube(name, localPosition, localScale, PartyArtMaterials.Get(materialType));
    }

    private GameObject AddCube(string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.name = name;
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

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
