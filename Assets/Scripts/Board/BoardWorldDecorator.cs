using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardWorldDecorator
{
    private readonly Transform root;
    private readonly System.Random random;
    private readonly List<Vector3> blockedPositions;

    public BoardWorldDecorator(Transform root, IReadOnlyList<BoardTile> tiles, int seed)
    {
        this.root = root;
        random = new System.Random(seed);
        blockedPositions = new List<Vector3>();
        foreach (var tile in tiles)
        {
            blockedPositions.Add(tile.transform.position);
        }
    }

    public void Build()
    {
        CreateSeaBase();
        CreateIslandBase();
        CreateWaterTiles();
        CreateProps(34);
        CreateCornerLanterns();
    }

    public void CreatePathConnectors(IReadOnlyList<BoardTile> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Vector3 a = tiles[i].transform.position;
            Vector3 b = tiles[(i + 1) % tiles.Count].transform.position;
            CreateConnector(a, b);
        }
    }

    private void CreateIslandBase()
    {
        var foundationObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        foundationObject.name = "Island Foundation Shadow";
        foundationObject.transform.SetParent(root);
        foundationObject.transform.position = new Vector3(0f, -0.29f, 0f);
        foundationObject.transform.localScale = new Vector3(7.75f, 0.16f, 10.05f);
        foundationObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Island Foundation", new Color(0.20f, 0.29f, 0.27f));
        RemoveCollider(foundationObject);

        var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObject.name = "Soft Island Base";
        baseObject.transform.SetParent(root);
        baseObject.transform.position = new Vector3(0f, -0.13f, 0f);
        baseObject.transform.localScale = new Vector3(7.0f, 0.28f, 9.28f);
        baseObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Island Grass", new Color(0.24f, 0.56f, 0.38f));
        RemoveCollider(baseObject);

        var shoreObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shoreObject.name = "Warm Shore Trim";
        shoreObject.transform.SetParent(root);
        shoreObject.transform.position = new Vector3(0f, -0.19f, 0f);
        shoreObject.transform.localScale = new Vector3(7.42f, 0.18f, 9.70f);
        shoreObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Shore Trim", new Color(0.73f, 0.65f, 0.45f));
        RemoveCollider(shoreObject);

        var plateauObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plateauObject.name = "Central Playfield Plateau";
        plateauObject.transform.SetParent(root);
        plateauObject.transform.position = new Vector3(0f, -0.015f, 0f);
        plateauObject.transform.localScale = new Vector3(5.92f, 0.05f, 8.08f);
        plateauObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Central Plateau", new Color(0.31f, 0.64f, 0.43f));
        RemoveCollider(plateauObject);
    }

    private void CreateSeaBase()
    {
        var seaObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seaObject.name = "Calm Sea Backdrop";
        seaObject.transform.SetParent(root);
        seaObject.transform.position = new Vector3(0f, -0.36f, 0f);
        seaObject.transform.localScale = new Vector3(12.8f, 0.06f, 14.2f);
        seaObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Calm Sea", new Color(0.09f, 0.31f, 0.43f));
        RemoveCollider(seaObject);
    }

    private void CreateWaterTiles()
    {
        for (int i = 0; i < 20; i++)
        {
            var water = GameObject.CreatePrimitive(PrimitiveType.Cube);
            water.name = "Low Poly Water Patch";
            water.transform.SetParent(root);
            float side = i % 2 == 0 ? -1f : 1f;
            float x = side * RandomRange(4.1f, 5.8f);
            float z = RandomRange(-5.3f, 5.3f);
            water.transform.position = new Vector3(x, -0.315f, z);
            water.transform.localScale = new Vector3(RandomRange(0.8f, 1.8f), 0.025f, RandomRange(0.42f, 1.18f));
            water.transform.rotation = Quaternion.Euler(0f, RandomRange(-18f, 18f), 0f);
            water.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Water Patch", new Color(0.18f, 0.52f, 0.66f, 1f));
            RemoveCollider(water);
        }
    }

    private void CreateProps(int count)
    {
        int attempts = 0;
        int placed = 0;
        while (placed < count && attempts < count * 6)
        {
            attempts++;
            Vector3 position = new Vector3(RandomRange(-3.0f, 3.0f), 0f, RandomRange(-4.2f, 4.2f));
            if (IsBlocked(position))
            {
                continue;
            }

            int propType = random.Next(0, 4);
            if (propType == 0)
            {
                CreateTree(position);
            }
            else if (propType == 1)
            {
                CreateRock(position);
            }
            else if (propType == 2)
            {
                CreateCrystal(position);
            }
            else
            {
                CreateFlag(position);
            }

            placed++;
        }
    }

    private void CreateConnector(Vector3 a, Vector3 b)
    {
        Vector3 center = (a + b) * 0.5f;
        Vector3 direction = b - a;
        var connector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        connector.name = "Board Path Segment";
        connector.transform.SetParent(root);
        connector.transform.position = new Vector3(center.x, 0.02f, center.z);
        connector.transform.localScale = new Vector3(0.36f, 0.055f, direction.magnitude);
        connector.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        connector.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Path Sand", new Color(0.79f, 0.71f, 0.50f));
        RemoveCollider(connector);

        var core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "Board Path Highlight";
        core.transform.SetParent(root);
        core.transform.position = new Vector3(center.x, 0.07f, center.z);
        core.transform.localScale = new Vector3(0.15f, 0.025f, direction.magnitude * 0.94f);
        core.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        core.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Path Highlight", new Color(0.96f, 0.88f, 0.62f));
        RemoveCollider(core);
    }

    private void CreateTree(Vector3 position)
    {
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Tiny Tree Trunk";
        trunk.transform.SetParent(root);
        trunk.transform.position = position + new Vector3(0f, 0.22f, 0f);
        trunk.transform.localScale = new Vector3(0.08f, 0.22f, 0.08f);
        trunk.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tree Trunk", new Color(0.45f, 0.29f, 0.18f));
        RemoveCollider(trunk);

        var top = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        top.name = "Tiny Tree Top";
        top.transform.SetParent(root);
        top.transform.position = position + new Vector3(0f, 0.62f, 0f);
        top.transform.localScale = Vector3.one * RandomRange(0.34f, 0.48f);
        top.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Tree Top", new Color(0.15f, 0.48f, 0.31f));
        RemoveCollider(top);
    }

    private void CreateRock(Vector3 position)
    {
        var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rock.name = "Smooth Pebble";
        rock.transform.SetParent(root);
        rock.transform.position = position + new Vector3(0f, 0.12f, 0f);
        rock.transform.localScale = new Vector3(RandomRange(0.28f, 0.52f), RandomRange(0.16f, 0.30f), RandomRange(0.22f, 0.48f));
        rock.transform.rotation = Quaternion.Euler(0f, RandomRange(0f, 180f), 0f);
        rock.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Pebble", new Color(0.50f, 0.56f, 0.58f));
        RemoveCollider(rock);
    }

    private void CreateCrystal(Vector3 position)
    {
        var crystal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crystal.name = "Little Crystal";
        crystal.transform.SetParent(root);
        crystal.transform.position = position + new Vector3(0f, 0.22f, 0f);
        crystal.transform.localScale = new Vector3(0.22f, RandomRange(0.36f, 0.62f), 0.22f);
        crystal.transform.rotation = Quaternion.Euler(RandomRange(0f, 8f), RandomRange(0f, 180f), RandomRange(0f, 8f));
        crystal.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Crystal", new Color(0.40f, 0.82f, 0.88f));
        RemoveCollider(crystal);
    }

    private void CreateFlag(Vector3 position)
    {
        var pole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pole.name = "Map Flag Pole";
        pole.transform.SetParent(root);
        pole.transform.position = position + new Vector3(0f, 0.32f, 0f);
        pole.transform.localScale = new Vector3(0.06f, 0.64f, 0.06f);
        pole.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Flag Pole", new Color(0.30f, 0.25f, 0.20f));
        RemoveCollider(pole);

        var flag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        flag.name = "Map Flag Cloth";
        flag.transform.SetParent(root);
        flag.transform.position = position + new Vector3(0.18f, 0.54f, 0f);
        flag.transform.localScale = new Vector3(0.34f, 0.20f, 0.035f);
        flag.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Flag Cloth", new Color(0.96f, 0.48f, 0.30f));
        RemoveCollider(flag);
    }

    private void CreateCornerLanterns()
    {
        Vector3[] positions =
        {
            new Vector3(-3.20f, 0f, -4.34f),
            new Vector3(3.20f, 0f, -4.34f),
            new Vector3(-3.20f, 0f, 4.34f),
            new Vector3(3.20f, 0f, 4.34f)
        };

        foreach (Vector3 position in positions)
        {
            var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObject.name = "Corner Lantern Base";
            baseObject.transform.SetParent(root);
            baseObject.transform.position = position + new Vector3(0f, 0.18f, 0f);
            baseObject.transform.localScale = new Vector3(0.16f, 0.18f, 0.16f);
            baseObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Lantern Base", new Color(0.16f, 0.19f, 0.21f));
            RemoveCollider(baseObject);

            var glowObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glowObject.name = "Corner Lantern Glow";
            glowObject.transform.SetParent(root);
            glowObject.transform.position = position + new Vector3(0f, 0.52f, 0f);
            glowObject.transform.localScale = Vector3.one * 0.22f;
            glowObject.GetComponent<Renderer>().material = RuntimeVisuals.CreateMaterial("Lantern Glow", new Color(1f, 0.80f, 0.36f));
            RemoveCollider(glowObject);
        }
    }

    private bool IsBlocked(Vector3 position)
    {
        foreach (Vector3 blocked in blockedPositions)
        {
            if (Vector3.Distance(position, blocked) < 0.85f)
            {
                return true;
            }
        }

        return false;
    }

    private float RandomRange(float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }

    private void RemoveCollider(GameObject gameObject)
    {
        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            UnityEngine.Object.Destroy(collider);
        }
    }
}
