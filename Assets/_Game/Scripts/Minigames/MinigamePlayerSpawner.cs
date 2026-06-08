using System.Collections.Generic;
using UnityEngine;

public class MinigamePlayerSpawner : MonoBehaviour
{
    private readonly List<PlayerMinigameController> spawnedPlayers = new List<PlayerMinigameController>();

    public IReadOnlyList<PlayerMinigameController> SpawnedPlayers => spawnedPlayers;

    public IReadOnlyList<PlayerMinigameController> SpawnPlayers(IReadOnlyList<PlayerMinigameData> players)
    {
        ClearSpawnedPlayers();
        EnsureSpawnPoints();
        PlayerSpawnPoint[] spawnPoints = GetComponentsInChildren<PlayerSpawnPoint>();

        for (int i = 0; i < players.Count; i++)
        {
            PlayerSpawnPoint point = FindSpawnPoint(spawnPoints, i);
            Vector3 position = point != null ? point.transform.position : transform.position + new Vector3(-1.8f + i * 1.2f, 0f, 0f);
            GameObject avatar = new GameObject($"{players[i].PlayerName} Template Avatar");
            avatar.transform.position = position;

            PlayerVisualController visual = avatar.AddComponent<PlayerVisualController>();
            visual.Configure(players[i].PlayerId, players[i].PlayerColor);

            PlayerMinigameController controller = avatar.AddComponent<PlayerMinigameController>();
            controller.Initialize(players[i]);
            players[i].AvatarObject = avatar;
            players[i].Controller = controller;
            spawnedPlayers.Add(controller);
        }

        return spawnedPlayers;
    }

    public void ClearSpawnedPlayers()
    {
        for (int i = spawnedPlayers.Count - 1; i >= 0; i--)
        {
            if (spawnedPlayers[i] != null)
            {
                Destroy(spawnedPlayers[i].gameObject);
            }
        }

        spawnedPlayers.Clear();
    }

    private void EnsureSpawnPoints()
    {
        if (GetComponentsInChildren<PlayerSpawnPoint>().Length > 0)
        {
            return;
        }

        Vector3[] positions =
        {
            new Vector3(-1.8f, 0.05f, -1.1f),
            new Vector3(-0.6f, 0.05f, -1.1f),
            new Vector3(0.6f, 0.05f, -1.1f),
            new Vector3(1.8f, 0.05f, -1.1f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            var pointObject = new GameObject($"P{i + 1} Spawn");
            pointObject.transform.SetParent(transform, false);
            pointObject.transform.localPosition = positions[i];
            pointObject.AddComponent<PlayerSpawnPoint>().Configure(i);
        }
    }

    private static PlayerSpawnPoint FindSpawnPoint(IEnumerable<PlayerSpawnPoint> spawnPoints, int playerIndex)
    {
        foreach (PlayerSpawnPoint point in spawnPoints)
        {
            if (point != null && point.PlayerIndex == playerIndex)
            {
                return point;
            }
        }

        return null;
    }
}
