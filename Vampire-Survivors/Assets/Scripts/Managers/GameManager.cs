using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;
using UnityEngine.Events;
using System;

// then there could be like a formula you could derive from this data
public struct PlayerScoreInfo
{
    public EntityPlayerType type;
    public Sprite spirte;
    public int killCount;
    public float timeSurvived;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private float _monotonicTimer;
    private bool _isPaused = false;
    private bool _showingFinalScoreInfo = false;

    public UnityEvent EventControllablePlayerIsDead;

    [Header("UI Refs")]
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private GameObject _playerScoreInfoPrefab;
    [SerializeField] private GameObject _finalScoreUI;

    // NOTE(Jovanni):
    // Using a linked list here for enemy list because there are lots of insertions and delations
    public List<EntityPlayer> PlayerList { get; set; }
    public LinkedList<EntityEnemy> EnemyList { get; set; }

    public List<PlayerScoreInfo> PlayerScores { get; set; }

    private Entity _controllablePlayer;
    private List<Vector3> _playableArea;
  

    public bool IsPaused() => _isPaused;
    public List<Vector3> GetPlayableArea() => _playableArea;
    public float GetMonotonicTime() => _monotonicTimer;

    public void ResumeGame()
    {
        _pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void PauseGame()
    {
        _pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
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

        PlayerScores  = new List<PlayerScoreInfo>();
        PlayerList = new List<EntityPlayer>();
        EnemyList = new LinkedList<EntityEnemy>();
    }

    private void Start()
    {
        _playableArea = new List<Vector3>();

        int bounds = 8;
        var boundsRect = _groundTilemap.cellBounds;

        for (int x = boundsRect.xMin + bounds; x < boundsRect.xMax - bounds; x++)
        {
            for (int y = boundsRect.yMin + bounds; y < boundsRect.yMax - bounds; y++)
            {
                Vector3Int localPlace = new Vector3Int(x, y, 0);
                if (_groundTilemap.HasTile(localPlace))
                {
                    Vector3 worldPos = _groundTilemap.CellToWorld(localPlace) + new Vector3(0.5f, 0.5f, 0f);
                    _playableArea.Add(worldPos);
                }
            }
        }

        _controllablePlayer = PlayerSpawner.Instance.SpawnPlayer(false).GetComponent<EntityPlayer>();
        _cinemachineCamera.Follow = _controllablePlayer.transform;
        _cinemachineCamera.LookAt = _controllablePlayer.transform;

        PlayerSpawner.Instance.SpawnPlayer(true);
        PlayerSpawner.Instance.SpawnPlayer(true);
        PlayerSpawner.Instance.SpawnPlayer(true);
        PlayerSpawner.Instance.SpawnPlayer(true);
        PlayerSpawner.Instance.SpawnPlayer(true);
    }

    void Update()
    {
        if (_showingFinalScoreInfo) return;
        if (PlayerList.Count == 0)
        {
            _finalScoreUI.SetActive(true);
            foreach (PlayerScoreInfo info in PlayerScores)
            {
                
                GameObject scoreCard = Instantiate(_playerScoreInfoPrefab, _finalScoreUI.transform);
                ScoreCardInfo uiInfo = scoreCard.GetComponent<ScoreCardInfo>();
                uiInfo.image.sprite = info.spirte;
                uiInfo.nameText.text = "Name: " + info.type.ToString();
                uiInfo.killedText.text = "Killed: " + info.killCount;
                uiInfo.timeText.text = "Time Survived: " + info.timeSurvived.ToString("0.00");
            }

            _showingFinalScoreInfo = true;
            Time.timeScale = 0.0f;
            return;
        }

        _monotonicTimer += Time.deltaTime;




        // TODO(Jovanni):
        // Ensure correctness
        // for example does adding a component call awake?
        if (_controllablePlayer.IsDead())
        {
            EventControllablePlayerIsDead?.Invoke();
        }

        if (_cinemachineCamera.Follow == null)
        {
            EntityPlayer entityToFollow = PlayerList[UnityEngine.Random.Range(0, PlayerList.Count)];
            FollowPositionTarget follow = GetComponent<FollowPositionTarget>();
            AIController controller = GetComponent<AIController>();
            Destroy(follow);
            Destroy(controller);

            entityToFollow.gameObject.AddComponent<PlayerController>();
            _controllablePlayer = entityToFollow.GetComponent<EntityPlayer>();
            _cinemachineCamera.Follow = entityToFollow.transform;
            _cinemachineCamera.LookAt = entityToFollow.transform;
        }

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
