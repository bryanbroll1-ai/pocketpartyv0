using System.Collections.Generic;
using UnityEngine;

public static class MinigamePlayerSpawnSystem
{
    private const string SpawnRootName = "Minigame Player Spawns";

    public static IReadOnlyList<GameObject> SpawnPlayers(IReadOnlyList<PlayerController> players, Vector3 center, float spacing)
    {
        Clear();

        var spawnedPlayers = new List<GameObject>();
        var root = new GameObject(SpawnRootName).transform;
        int count = players != null ? players.Count : 0;
        float startX = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            PlayerController player = players[i];
            Vector3 position = center + new Vector3(startX + i * spacing, 0f, 0f);
            GameObject avatar = CreateAvatar(player, position);
            avatar.transform.SetParent(root);
            spawnedPlayers.Add(avatar);
        }

        return spawnedPlayers;
    }

    public static void Clear()
    {
        GameObject existing = GameObject.Find(SpawnRootName);
        if (existing != null)
        {
            Object.Destroy(existing);
        }
    }

    private static GameObject CreateAvatar(PlayerController player, Vector3 position)
    {
        var avatar = new GameObject($"{player.PlayerName} Minigame Avatar");
        avatar.transform.position = position;

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(avatar.transform, false);
        body.transform.localPosition = new Vector3(0f, 0.40f, 0f);
        body.transform.localScale = new Vector3(0.28f, 0.34f, 0.28f);
        body.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial($"{player.PlayerName} Minigame Body", player.PlayerColor);
        RemoveCollider(body);

        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(avatar.transform, false);
        head.transform.localPosition = new Vector3(0f, 0.86f, 0f);
        head.transform.localScale = Vector3.one * 0.24f;
        head.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial($"{player.PlayerName} Minigame Head", new Color(0.94f, 0.78f, 0.58f));
        RemoveCollider(head);

        var shadow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shadow.name = "Shadow";
        shadow.transform.SetParent(avatar.transform, false);
        shadow.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        shadow.transform.localScale = new Vector3(0.42f, 0.02f, 0.32f);
        shadow.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Avatar Shadow", new Color(0f, 0f, 0f, 0.45f));
        RemoveCollider(shadow);

        return avatar;
    }

    private static void RemoveCollider(GameObject target)
    {
        var collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Object.Destroy(collider);
        }
    }
}
