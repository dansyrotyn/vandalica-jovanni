using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Random = UnityEngine.Random;

public class EnemyWaveSpawner : MonoBehaviour
{
    public static EnemyWaveSpawner Instance { get; private set; }

    [Header("Spawner Stats")]
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _spawnCooldownTime = 5.0f;
    [SerializeField] private float _spawnCurrnetCooldownTime = 0.0f;

    private int _waveNumber = 0;
    private float _blueGreenColor = 1.0f;
    private float _additionalAnimationSpeed = 0.1f;
    private float _additionalMovementSpeed = 0.1f;

    public int GetWaveNumber() =>  _waveNumber;
    public bool IsOnCooldown() => _spawnCurrnetCooldownTime > 0.0f;

    void SpawnOnGroundLayer()
    {
        List<Vector3> playableArea = GameState.Instance.GetPlayableArea();

        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 worldPos = playableArea[Random.Range(0, playableArea.Count)];
            GameObject skeletonObject = Instantiate(_entityPrefab, worldPos, Quaternion.identity, transform);
            SkeletonController skeleton = skeletonObject.GetComponent<SkeletonController>();
            SpriteRenderer skeletonRenderer = skeleton.GetComponent<SpriteRenderer>();
            Animator skeletonAnim = skeleton.GetComponent<Animator>();
            Color skeletonColor = skeletonRenderer.color;
            skeletonColor.r = 1.0f;
            skeletonColor.g = _blueGreenColor;
            skeletonColor.b = _blueGreenColor;

            skeleton.AddSpeed(_additionalMovementSpeed);
            skeletonAnim.speed += _additionalAnimationSpeed;
            skeletonRenderer.color = skeletonColor;
        }
    }

    public void SpawnNextWave()
    {
        if (IsOnCooldown()) return;

        SpawnOnGroundLayer();
        _waveNumber += 1;
        _spawnCount += 10;
        _additionalAnimationSpeed *= 1.1f;
        _additionalMovementSpeed *= 1.1f;
        _blueGreenColor = Math.Max(_blueGreenColor - 0.1f, 0f);

        _spawnCurrnetCooldownTime = _spawnCooldownTime;
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
    }
    

    void Update()
    {
        _spawnCurrnetCooldownTime = (float)Math.Max(_spawnCurrnetCooldownTime - Time.deltaTime, 0.0f);
    }
}
