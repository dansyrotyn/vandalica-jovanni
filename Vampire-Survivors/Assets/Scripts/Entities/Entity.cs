using UnityEngine;
using UnityEngine.VFX;

public abstract class Entity : MonoBehaviour, IDamagable
{
    protected Rigidbody2D _rb;
    protected EntityVisualHandler _visual;

    [Header("Entity Info")]
    [SerializeField] protected int _health;
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected bool _isDead;

    public abstract void Damage(int dmg);
    public bool IsDead() => _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _visual = GetComponent<EntityVisualHandler>();
        _health = _maxHealth;
    }
}

public enum EntityPlayerType
{
    KNIGHT,
    WIZARD
}

public abstract class EntityPlayer : Entity
{
    [Header("Player Entity Info")]
    [SerializeField] private Sprite _sprite;

    public int EnemyKillCount { set; get; }
    protected EntityPlayerType _type;

    void OnDestroy()
    {
        PlayerScoreInfo info = new PlayerScoreInfo();
        info.type = _type;
        info.texture = _sprite.texture;
        info.killCount = EnemyKillCount;
        info.timeSurvived = GameManager.Instance.GetMonotonicTime();
        GameManager.Instance.PlayerScores.Add(info);
    }
}

public enum EntityEnemyType
{
    SKELETON,
}

public abstract class EntityEnemy : Entity
{
    public EntityEnemyType type;
}