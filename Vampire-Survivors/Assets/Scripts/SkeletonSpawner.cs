using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

public class SkeletonSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _spawnCooldownTime = 5.0f;
    [SerializeField] private float _spawnCurrnetCooldownTime = 0.0f;
    private List<Vector3Int> _groundCells;

    void Start()
    {
        _groundCells = new List<Vector3Int>();
        BoundsInt bounds = _groundTilemap.cellBounds;

        for (int x = bounds.xMin + 1; x < bounds.xMax - 1; x++)
        {
            for (int y = bounds.yMin + 1; y < bounds.yMax - 1; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (_groundTilemap.HasTile(cell))
                {
                    _groundCells.Add(cell);
                }
            }
        }
    }

    void Update()
    {
        if (_spawnCurrnetCooldownTime == 0.0)
        {
            SpawnOnGroundLayer();
            _spawnCurrnetCooldownTime = _spawnCooldownTime;
            _spawnCount += 10;
        }
        else
        {
            _spawnCurrnetCooldownTime = (float)Math.Max(_spawnCurrnetCooldownTime - Time.deltaTime, 0.0f);
        }
    }

    void SpawnOnGroundLayer()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3Int cell = _groundCells[Random.Range(0, _groundCells.Count - 1)];
            Vector3 worldPos = _groundTilemap.CellToWorld(cell) + _groundTilemap.tileAnchor;
            Instantiate(_entityPrefab, worldPos, Quaternion.identity, transform);
        }
    }
}
