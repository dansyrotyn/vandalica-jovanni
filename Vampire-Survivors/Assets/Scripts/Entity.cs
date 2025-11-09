using UnityEngine;

public class Entity : MonoBehaviour, IDamagable
{
    protected Rigidbody2D _rb;
    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
 
    [SerializeField] protected int _health;
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected bool _isDead;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _health = _maxHealth;
    }

    public void Damage(int dmg)
    {
        _health -= dmg;
    }
}