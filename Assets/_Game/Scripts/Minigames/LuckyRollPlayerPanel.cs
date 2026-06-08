using UnityEngine;

public class LuckyRollPlayerPanel : MonoBehaviour
{
    private TextMesh nameText;
    private TextMesh resultText;
    private RollingNumberDisplay display;
    private Color playerColor;

    public RollingNumberDisplay Display => display;

    public void Build(PlayerMinigameData player)
    {
        playerColor = player.PlayerColor;
        ClearChildren();
        AddCube("Player Color Zone", new Vector3(0f, 0.02f, 0f), new Vector3(1.55f, 0.10f, 1.62f), PartyArtMaterials.CreateTinted("Lucky Player Zone", playerColor));
        AddCube("Player Zone Inner", new Vector3(0f, 0.10f, 0f), new Vector3(1.28f, 0.05f, 1.34f), PartyArtMaterialType.UIPanel);
        AddCylinder("Stop Pad", new Vector3(0f, 0.18f, -0.42f), new Vector3(0.38f, 0.05f, 0.38f), PartyArtMaterials.CreateTinted("Lucky Stop Pad", playerColor));

        GameObject displayObject = new GameObject("Rolling Number Display");
        displayObject.transform.SetParent(transform, false);
        displayObject.transform.localPosition = new Vector3(0f, 1.78f, -0.22f);
        display = displayObject.AddComponent<RollingNumberDisplay>();
        display.Build(playerColor);

        nameText = CreateText("Player Label", new Vector3(0f, 0.30f, 0.62f), 42, 0.030f, PartyArtPalette.DarkAccent);
        resultText = CreateText("Result Label", new Vector3(0f, 0.30f, -0.78f), 34, 0.028f, PartyArtPalette.DarkAccent);
        nameText.text = player.PlayerName;
        resultText.text = "Tap!";
    }

    public void SetRolling()
    {
        resultText.text = "Tap!";
        resultText.color = PartyArtPalette.DarkAccent;
        display?.ResetRoll();
    }

    public void SetStopped(int value)
    {
        resultText.text = $"Rolled {value}";
        resultText.color = playerColor;
        display?.ShowFinal(value);
    }

    public void SetTieBreaker()
    {
        resultText.text = "Reroll!";
        resultText.color = PartyArtPalette.Lava;
        display?.ShowTieBreaker();
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
