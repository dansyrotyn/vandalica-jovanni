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

// This class really only exists for type safety
public abstract class EntityPlayer : Entity { }
public abstract class EntityEnemy : Entity{}