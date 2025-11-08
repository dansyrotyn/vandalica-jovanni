using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    private float _monotonicTimer;
    private bool _isPaused = false;

    [Header("UI Refs")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private Tilemap _groundTilemap;

    private List<Vector3> _playableArea;

    public bool IsPaused() => _isPaused;
    public List<Vector3> GetPlayableArea() => _playableArea;

    public void ResumeGame()
    {
        _pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Resumes time
        _isPaused = false;
    }

    public void PauseGame()
    {
        _pauseMenuPanel.SetActive(true);
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

    // rework this
    private void Start()
    {
        _playableArea = new List<Vector3>();

        BoundsInt bounds = _groundTilemap.cellBounds;
        for (int x = bounds.xMin + 2; x < bounds.xMax - 2; x++)
        {
            for (int y = bounds.yMin + 2; y < bounds.yMax - 2; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (_groundTilemap.HasTile(cell))
                {
                    Vector3 worldPos = _groundTilemap.GetCellCenterWorld(cell);
                    _playableArea.Add(worldPos);
                }
            }
        }
    }


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
