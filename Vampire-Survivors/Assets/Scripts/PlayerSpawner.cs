using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [SerializeField] private List<GameObject> _playerPrefabs;

    private List<Transform> _spawnPoints;
    private int _spawnIndex;

    // want to change bevavior to make it have ai behavior
    public GameObject SpawnPlayer()
    {
        GameObject prefab = PickRandomPlayerPrefab();
        Transform spawnPoint = PickRandomSpawnPoint();

        GameObject entity = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        // entity.Initalize(behavior);

        return entity;
    }

    private GameObject PickRandomPlayerPrefab()
    {
        int index = UnityEngine.Random.Range(0, _playerPrefabs.Count);
        return _playerPrefabs[index];
    }

    private Transform PickRandomSpawnPoint()
    {
        return _spawnPoints[_spawnIndex++];
    }

    void Start()
    {
        _spawnPoints = new List<Transform>();

        foreach (Transform child in this.transform)
        {
            _spawnPoints.Add(child);
        }
    }

    void Update()
    {
        
    }
}
