using UnityEngine;

public class PlayerTowerMeter : MonoBehaviour
{
    private Transform fill;
    private Transform cap;
    private TextMesh scoreText;
    private TextMesh nameText;
    private float maxHeight = 3.45f;
    private Color playerColor;

    public void Build(PlayerMinigameData player, float visualMaxHeight)
    {
        playerColor = player.PlayerColor;
        maxHeight = visualMaxHeight;
        ClearChildren();

        AddCube("Tower Backplate", new Vector3(0f, 1.75f, 0.18f), new Vector3(0.88f, 3.65f, 0.18f), PartyArtMaterialType.WhiteAccent);
        AddCube("Tower Track", new Vector3(0f, 1.75f, 0.02f), new Vector3(0.54f, 3.36f, 0.16f), PartyArtMaterialType.UIPanel);
        fill = AddCube("Tower Fill", new Vector3(0f, 0.10f, -0.10f), new Vector3(0.46f, 0.08f, 0.28f), PartyArtMaterials.CreateTinted("Tower Fill", playerColor)).transform;
        cap = AddCube("Tower Cap", new Vector3(0f, 0.18f, -0.10f), new Vector3(0.62f, 0.14f, 0.34f), PartyArtMaterials.CreateTinted("Tower Cap", playerColor)).transform;
        AddCylinder("Tower Base Pad", new Vector3(0f, 0.05f, -0.28f), new Vector3(0.52f, 0.04f, 0.52f), PartyArtMaterials.CreateTinted("Tower Base", playerColor));

        nameText = CreateText("Tower Name", new Vector3(0f, 3.78f, -0.18f), 38, 0.028f, PartyArtPalette.DarkAccent);
        scoreText = CreateText("Tower Score", new Vector3(0f, 3.34f, -0.20f), 46, 0.030f, playerColor);
        nameText.text = player.PlayerName;
        SetHeight(0f, 0);
    }

    public void SetHeight(float height, int score)
    {
        float clamped = Mathf.Clamp(height, 0.08f, maxHeight);
        if (fill != null)
        {
            fill.localScale = new Vector3(0.46f, clamped, 0.28f);
            fill.localPosition = new Vector3(0f, clamped * 0.5f, -0.10f);
        }

        if (cap != null)
        {
            cap.localPosition = new Vector3(0f, clamped + 0.08f, -0.10f);
        }

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void Pulse()
    {
        if (cap != null)
        {
            cap.localScale = new Vector3(0.72f, 0.18f, 0.40f);
        }
    }

    public void Relax()
    {
        if (cap != null)
        {
            cap.localScale = new Vector3(0.62f, 0.14f, 0.34f);
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
        RemoveCollider(instance);
        return instance;
    }

    private GameObject AddCylinder(string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        instance.name = name;
        instance.transform.SetParent(transform, false);
        instance.transform.localPosition = localPosition;
        instance.transform.localScale = localScale;
        instance.GetComponent<Renderer>().material = material;
        RemoveCollider(instance);
        return instance;
    }

    private static void RemoveCollider(GameObject instance)
    {
        Collider collider = instance.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
