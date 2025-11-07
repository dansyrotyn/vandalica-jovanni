using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

public class SkeletonSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private GameObject entityPrefab;
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnCooldownTime = 5.0f;
    [SerializeField] private float spawnCurrnetCooldownTime = 0.0f;
    private List<Vector3Int> groundCells;

    void Start()
    {
        this.groundCells = new List<Vector3Int>();
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin + 1; x < bounds.xMax - 1; x++)
        {
            for (int y = bounds.yMin + 1; y < bounds.yMax - 1; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (groundTilemap.HasTile(cell))
                {
                    groundCells.Add(cell);
                }
            }
        }
    }

    void Update()
    {
        if (spawnCurrnetCooldownTime == 0.0)
        {
            spawnOnGroundLayer();
            spawnCurrnetCooldownTime = spawnCooldownTime;
            spawnCount += 10;
        }
        else
        {
            spawnCurrnetCooldownTime = (float)Math.Max(spawnCurrnetCooldownTime - Time.deltaTime, 0.0f);
        }
    }

    void spawnOnGroundLayer()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3Int cell = groundCells[Random.Range(0, groundCells.Count)];
            Vector3 worldPos = groundTilemap.CellToWorld(cell) + groundTilemap.tileAnchor;
            Instantiate(entityPrefab, worldPos, Quaternion.identity, this.transform);
        }
    }
}
