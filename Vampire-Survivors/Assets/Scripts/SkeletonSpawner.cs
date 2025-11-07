using System;
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
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3Int cell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            Vector3 worldPos = groundTilemap.CellToWorld(cell) + groundTilemap.tileAnchor;
            Instantiate(entityPrefab, worldPos, Quaternion.identity, this.gameObject.transform);
        }
    }
}
