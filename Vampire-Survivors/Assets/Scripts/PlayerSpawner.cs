using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    [SerializeField] private List<GameObject> _prefabs;

    private List<Transform> _spawnPoints;
    private int _spawnIndex;

    public GameObject SpawnPlayer(bool useAI)
    {
        GameObject prefab = PickRandomPlayerPrefab();
        Transform spawnPoint = PickRandomSpawnPoint();

        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        if (useAI)
        {
            obj.AddComponent<AIController>();
            FollowPositionTarget follow = obj.AddComponent<FollowPositionTarget>();
            follow.SetSpeed(5.0f);
        }
        else
        {
            obj.AddComponent<PlayerController>();
        }

        GameState.Instance.PlayerList.Add(obj);

        return obj;
    }

    private GameObject PickRandomPlayerPrefab()
    {
        int index = UnityEngine.Random.Range(0, _prefabs.Count);
        return _prefabs[index];
    }

    private Transform PickRandomSpawnPoint()
    {
        if (_spawnIndex >= _spawnPoints.Count)
        {
            Debug.LogError("PickRandomSpawnPoint() Index out of bounds! Returning null.");
            return null;
        }

        return _spawnPoints[_spawnIndex++];
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _spawnPoints = new List<Transform>();
        foreach (Transform child in this.transform)
        {
            _spawnPoints.Add(child);
        }

        _prefabs = Resources.LoadAll<GameObject>("Prefab/Hero").ToList();
    }
}
