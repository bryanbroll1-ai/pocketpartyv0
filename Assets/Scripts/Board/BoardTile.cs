using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public int Index { get; private set; }
    public BoardTileType TileType { get; private set; }

    public void Initialize(int index, BoardTileType tileType, Vector3 position)
    {
        Index = index;
        TileType = tileType;
        transform.position = position;
        name = $"Tile {index:00} {tileType}";

        CreateTileBody();
    }

    private void CreateTileBody()
    {
        var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObject.name = "Tile Base";
        baseObject.transform.SetParent(transform, false);
        baseObject.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        baseObject.transform.localScale = new Vector3(0.58f, 0.08f, 0.58f);
        baseObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tile Side", GetSideColor(TileType));
        RemoveCollider(baseObject);

        var capObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        capObject.name = "Tile Color Cap";
        capObject.transform.SetParent(transform, false);
        capObject.transform.localPosition = new Vector3(0f, 0.20f, 0f);
        capObject.transform.localScale = new Vector3(0.52f, 0.025f, 0.52f);
        capObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tile Cap", GetTileColor(TileType));
        RemoveCollider(capObject);

        var rimObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rimObject.name = "Tile Soft Rim";
        rimObject.transform.SetParent(transform, false);
        rimObject.transform.localPosition = new Vector3(0f, 0.235f, 0f);
        rimObject.transform.localScale = new Vector3(0.58f, 0.012f, 0.58f);
        rimObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tile Rim", GetRimColor(TileType));
        RemoveCollider(rimObject);

        var centerObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        centerObject.name = "Tile Center Inlay";
        centerObject.transform.SetParent(transform, false);
        centerObject.transform.localPosition = new Vector3(0f, 0.255f, 0f);
        centerObject.transform.localScale = GetInlayScale(TileType);
        centerObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tile Center Inlay", GetInlayColor(TileType));
        RemoveCollider(centerObject);

        if (TileType == BoardTileType.Minigame || TileType == BoardTileType.Event)
        {
            var beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            beacon.name = "Special Tile Beacon";
            beacon.transform.SetParent(transform, false);
            beacon.transform.localPosition = new Vector3(0f, 0.43f, 0f);
            beacon.transform.localScale = TileType == BoardTileType.Minigame ? new Vector3(0.20f, 0.08f, 0.20f) : new Vector3(0.14f, 0.22f, 0.14f);
            Color highlight = GetHighlightColor(TileType);
            beacon.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterialAdvanced("Special Beacon", highlight, 0.6f, 0f, highlight * 0.7f);
            beacon.AddComponent<PulseEmissive>().Configure(2.6f, 0.8f, 0.14f);
            beacon.AddComponent<BobMotion>().Configure(0.04f, 1.9f, 0f);
            RemoveCollider(beacon);
        }
    }

    private Color GetTileColor(BoardTileType tileType)
    {
        switch (tileType)
        {
            case BoardTileType.CoinPlus:
                return new Color(0.24f, 0.76f, 0.39f);
            case BoardTileType.CoinMinus:
                return new Color(0.91f, 0.33f, 0.30f);
            case BoardTileType.Event:
                return new Color(0.98f, 0.76f, 0.25f);
            case BoardTileType.Minigame:
                return new Color(0.35f, 0.64f, 0.95f);
            default:
                return new Color(0.86f, 0.88f, 0.84f);
        }
    }

    private Color GetSideColor(BoardTileType tileType)
    {
        Color topColor = GetTileColor(tileType);
        return new Color(topColor.r * 0.78f, topColor.g * 0.78f, topColor.b * 0.78f, 1f);
    }

    private Color GetRimColor(BoardTileType tileType)
    {
        Color topColor = GetTileColor(tileType);
        return new Color(Mathf.Min(1f, topColor.r + 0.12f), Mathf.Min(1f, topColor.g + 0.12f), Mathf.Min(1f, topColor.b + 0.12f), 1f);
    }

    private Color GetInlayColor(BoardTileType tileType)
    {
        switch (tileType)
        {
            case BoardTileType.CoinPlus:
            case BoardTileType.CoinMinus:
                return new Color(1f, 0.92f, 0.44f);
            case BoardTileType.Event:
                return new Color(0.17f, 0.16f, 0.12f);
            case BoardTileType.Minigame:
                return new Color(0.92f, 0.97f, 1f);
            default:
                return new Color(0.70f, 0.74f, 0.70f);
        }
    }

    private Color GetHighlightColor(BoardTileType tileType)
    {
        switch (tileType)
        {
            case BoardTileType.Event:
                return new Color(1f, 0.88f, 0.34f);
            case BoardTileType.Minigame:
                return new Color(0.62f, 0.86f, 1f);
            default:
                return GetTileColor(tileType);
        }
    }

    private Vector3 GetInlayScale(BoardTileType tileType)
    {
        switch (tileType)
        {
            case BoardTileType.CoinPlus:
                return new Vector3(0.22f, 0.008f, 0.22f);
            case BoardTileType.CoinMinus:
                return new Vector3(0.34f, 0.008f, 0.12f);
            case BoardTileType.Event:
                return new Vector3(0.18f, 0.010f, 0.18f);
            case BoardTileType.Minigame:
                return new Vector3(0.30f, 0.010f, 0.30f);
            default:
                return new Vector3(0.16f, 0.006f, 0.16f);
        }
    }

    private void RemoveCollider(GameObject target)
    {
        var collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }
}
