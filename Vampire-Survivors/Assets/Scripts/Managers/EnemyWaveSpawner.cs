using Coherence;
using Coherence.Toolkit;
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemyWaveSpawner : MonoBehaviour
{
    public static EnemyWaveSpawner Instance { get; private set; }

    [Header("Spawner Stats")]
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _spawnCooldownTime = 5.0f;
    [SerializeField] private int _maxEnemiesCount = 400;
    [SerializeField] private float _maxUnitsSpeed = 1.0f;

    public int WaveNumber { get; set; } = 0;
    public float SpawnCurrnetCooldownTime { get; set; } = 0.0f;

    protected CoherenceSync networkSync;
    public bool IsLocal => networkSync && networkSync.HasStateAuthority;

    private float _blueGreenColor = 1.0f;
    private float _movementSpeed = 1f;

    public int GetWaveNumber() => WaveNumber;
    public bool IsOnCooldown() => SpawnCurrnetCooldownTime > 0.0f;

    // expensive don't do this every frame!
    public List<GameObject> GetEnemies()
    {
        List<GameObject> enemies = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            enemies.Add(child.gameObject);
        }

        return enemies;
    }


    [Command()]

    public void SpawnOnGroundLayer(int toSpawn)
    {
        List<Vector3> playableArea = GameManager.Instance.GetPlayableArea();

        for (int i = 0; i < toSpawn; i++)
        {
            Vector3 worldPos = playableArea[Random.Range(0, playableArea.Count)];
            EntityEnemy enemy = Instantiate(_entityPrefab, worldPos, Quaternion.identity, transform).GetComponent<EntityEnemy>();
            SpriteRenderer skeletonRenderer = enemy.gameObject.GetComponent<SpriteRenderer>();
            Animator skeletonAnim = enemy.gameObject.GetComponent<Animator>();
            FollowGameObject follow = enemy.gameObject.GetComponent<FollowGameObject>();
            Color skeletonColor = skeletonRenderer.color;
            skeletonColor.r = 1.0f;
            skeletonColor.g = _blueGreenColor;
            skeletonColor.b = _blueGreenColor;

            follow.SetSpeed(_movementSpeed);
            skeletonAnim.speed *= _movementSpeed;
            skeletonRenderer.color = skeletonColor;
        }
    }

    public void ResetSpawner()
    {
        WaveNumber = 0;
        _spawnCount = 10;
        _movementSpeed = 1f;
        _blueGreenColor = 1.0f;
        SpawnCurrnetCooldownTime = 0.0f;
    }

    public void SpawnNextWave(int clients = 1)
    {
        if (!IsLocal) return;

        if (IsOnCooldown()) return;

        if (clients <= 0) return;

        var currentEnemies = GameManager.Instance.EnemyList.Count;

        _spawnCount = Mathf.Min(_spawnCount, _maxEnemiesCount - currentEnemies);

        if (_spawnCount <= 0) return;

        var toSpawn = _spawnCount / clients;
        networkSync.SendCommand<EnemyWaveSpawner>(
        nameof(SpawnOnGroundLayer),
        MessageTarget.All, toSpawn);
        _spawnCount += 10;

        _movementSpeed *= 1.1f;
        if(_movementSpeed > _maxUnitsSpeed)
        {
            _movementSpeed = _maxUnitsSpeed;
        }
        _blueGreenColor = Math.Max(_blueGreenColor - 0.1f, 0f);

       
        SpawnCurrnetCooldownTime = _spawnCooldownTime;
        WaveNumber += 1;
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
            networkSync = GetComponent<CoherenceSync>();
        }
    }


    void Update()
    {
        if (IsLocal)
        {
            SpawnCurrnetCooldownTime = (float)Math.Max(SpawnCurrnetCooldownTime - Time.deltaTime, 0.0f);
        }
    }
}
