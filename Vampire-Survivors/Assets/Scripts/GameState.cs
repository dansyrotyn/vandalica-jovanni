using System;
using TMPro;
using Unity.Burst;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    private float _monotonicTimer;
    private bool _isPaused = false;

    [Header("UI Refs")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _waveText;

    public void ResumeGame()
    {
        // _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resumes time
        _isPaused = false;
    }

    void PauseGame()
    {
        // _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pauses time
        _isPaused = true;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _monotonicTimer += Time.deltaTime;

        // Note(Jovanni):
        // This is probably not great for performance 
        // because you are doing allocations of some kind every frame.
        _timeText.text = "Time: " + _monotonicTimer.ToString("0.00");

        if (!EnemyWaveSpawner.Instance.IsOnCooldown())
        {
            EnemyWaveSpawner.Instance.SpawnNextWave();
            _waveText.text = "Wave: " + EnemyWaveSpawner.Instance.GetWaveNumber();
        }
    }
}
