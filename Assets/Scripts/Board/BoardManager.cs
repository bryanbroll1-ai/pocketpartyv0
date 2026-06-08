using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public IReadOnlyList<BoardTile> Tiles => tiles;

    private readonly List<BoardTile> tiles = new List<BoardTile>();
    private readonly Dictionary<PlayerController, GameObject> playerMarkers = new Dictionary<PlayerController, GameObject>();
    private Transform boardRoot;
    private Transform markerRoot;
    private Transform dressingRoot;
    private GameObject activeRing;

    public void BuildBoard(IReadOnlyList<PlayerController> players)
    {
        ClearBoard();

        boardRoot = new GameObject("Generated Board").transform;
        markerRoot = new GameObject("Player Markers").transform;
        dressingRoot = new GameObject("Map Dressing").transform;

        var tileTypes = CreateTilePattern();
        const float width = 4.8f;
        const float height = 7.4f;
        for (int i = 0; i < tileTypes.Count; i++)
        {
            var tileObject = new GameObject($"Tile {i:00}");
            tileObject.transform.SetParent(boardRoot);

            var tile = tileObject.AddComponent<BoardTile>();
            tile.Initialize(i, tileTypes[i], GetBoardPosition(i, tileTypes.Count, width, height));
            tiles.Add(tile);
        }

        int seed = Mathf.RoundToInt(Time.realtimeSinceStartup * 1000f) + players.Count * 97;
        var decorator = new BoardWorldDecorator(dressingRoot, tiles, seed);
        decorator.Build();
        decorator.CreatePathConnectors(tiles);
        CreateActiveRing();

        foreach (var player in players)
        {
            var marker = CreateMarker(player);
            playerMarkers[player] = marker;
            SetPlayerTile(player, 0);
        }
    }

    public IEnumerator MovePlayer(PlayerController player, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            int nextIndex = (player.TileIndex + 1) % tiles.Count;
            Vector3 start = GetMarkerPosition(player, player.TileIndex);
            player.TileIndex = nextIndex;
            Vector3 end = GetMarkerPosition(player, nextIndex);
            yield return AnimateMarker(player, start, end);
            SetPlayerTile(player, nextIndex);
        }
    }

    public BoardTile GetTileForPlayer(PlayerController player)
    {
        if (tiles.Count == 0)
        {
            return null;
        }

        return tiles[Mathf.Clamp(player.TileIndex, 0, tiles.Count - 1)];
    }

    private void ClearBoard()
    {
        foreach (var marker in playerMarkers.Values)
        {
            if (marker != null)
            {
                Destroy(marker);
            }
        }

        playerMarkers.Clear();
        tiles.Clear();

        if (boardRoot != null)
        {
            Destroy(boardRoot.gameObject);
        }

        if (markerRoot != null)
        {
            Destroy(markerRoot.gameObject);
        }

        if (dressingRoot != null)
        {
            Destroy(dressingRoot.gameObject);
        }

        if (activeRing != null)
        {
            Destroy(activeRing);
        }
    }

    private List<BoardTileType> CreateTilePattern()
    {
        return new List<BoardTileType>
        {
            BoardTileType.Normal,
            BoardTileType.CoinPlus,
            BoardTileType.Normal,
            BoardTileType.CoinMinus,
            BoardTileType.Event,
            BoardTileType.CoinPlus,
            BoardTileType.Normal,
            BoardTileType.Minigame,
            BoardTileType.CoinPlus,
            BoardTileType.Normal,
            BoardTileType.CoinMinus,
            BoardTileType.Normal,
            BoardTileType.Event,
            BoardTileType.CoinPlus,
            BoardTileType.Normal,
            BoardTileType.Minigame,
            BoardTileType.CoinMinus,
            BoardTileType.Normal,
            BoardTileType.CoinPlus,
            BoardTileType.Event,
            BoardTileType.Normal,
            BoardTileType.CoinPlus,
            BoardTileType.CoinMinus,
            BoardTileType.Minigame
        };
    }

    private Vector3 GetBoardPosition(int index, int totalTiles, float width, float height)
    {
        float t = index / (float)totalTiles;
        if (t < 0.25f)
        {
            return new Vector3(Mathf.Lerp(-width * 0.5f, width * 0.5f, t / 0.25f), 0f, height * 0.5f);
        }

        if (t < 0.5f)
        {
            return new Vector3(width * 0.5f, 0f, Mathf.Lerp(height * 0.5f, -height * 0.5f, (t - 0.25f) / 0.25f));
        }

        if (t < 0.75f)
        {
            return new Vector3(Mathf.Lerp(width * 0.5f, -width * 0.5f, (t - 0.5f) / 0.25f), 0f, -height * 0.5f);
        }

        return new Vector3(-width * 0.5f, 0f, Mathf.Lerp(-height * 0.5f, height * 0.5f, (t - 0.75f) / 0.25f));
    }

    private GameObject CreateMarker(PlayerController player)
    {
        var marker = new GameObject($"{player.PlayerName} Marker");
        marker.transform.SetParent(markerRoot);

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Player Body";
        body.transform.SetParent(marker.transform, false);
        body.transform.localPosition = new Vector3(0f, 0.55f, 0f);
        body.transform.localScale = new Vector3(0.34f, 0.42f, 0.34f);
        body.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial($"{player.PlayerName} Body", player.PlayerColor);
        RemoveCollider(body);

        var face = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        face.name = "Player Face Dot";
        face.transform.SetParent(marker.transform, false);
        face.transform.localPosition = new Vector3(0f, 0.72f, -0.19f);
        face.transform.localScale = Vector3.one * 0.08f;
        face.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial($"{player.PlayerName} Face", Color.white);
        RemoveCollider(face);
        return marker;
    }

    private void SetPlayerTile(PlayerController player, int tileIndex)
    {
        if (!playerMarkers.TryGetValue(player, out GameObject marker) || tileIndex >= tiles.Count)
        {
            return;
        }

        marker.transform.position = GetMarkerPosition(player, tileIndex);
    }

    public Transform GetMarkerTransform(PlayerController player)
    {
        return playerMarkers.TryGetValue(player, out GameObject marker) ? marker.transform : null;
    }

    public void SetActivePlayer(PlayerController player)
    {
        if (activeRing == null || player == null || !playerMarkers.TryGetValue(player, out GameObject marker))
        {
            return;
        }

        activeRing.SetActive(true);
        activeRing.transform.SetParent(marker.transform, false);
        activeRing.transform.localPosition = new Vector3(0f, 0.06f, 0f);
    }

    private Vector3 GetMarkerPosition(PlayerController player, int tileIndex)
    {
        int offsetIndex = player.PlayerIndex % 4;
        Vector3 offset = new Vector3((offsetIndex % 2 == 0 ? -0.18f : 0.18f), 0.18f, (offsetIndex < 2 ? 0.18f : -0.18f));
        return tiles[tileIndex].transform.position + offset;
    }

    private IEnumerator AnimateMarker(PlayerController player, Vector3 start, Vector3 end)
    {
        if (!playerMarkers.TryGetValue(player, out GameObject marker))
        {
            yield break;
        }

        const float duration = 0.22f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            Vector3 position = Vector3.Lerp(start, end, eased);
            position.y += Mathf.Sin(t * Mathf.PI) * 0.32f;
            marker.transform.position = position;
            yield return null;
        }

        marker.transform.position = end;
    }

    private void CreateActiveRing()
    {
        activeRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        activeRing.name = "Current Turn Glow";
        activeRing.transform.localScale = new Vector3(0.64f, 0.025f, 0.64f);
        activeRing.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Current Turn Glow", new Color(1f, 0.95f, 0.42f, 1f));
        RemoveCollider(activeRing);
        activeRing.SetActive(false);
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
