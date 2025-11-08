using System.Collections;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerController _playerReference;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private float _speed = 2f;

    private bool _isDead = false;
    private bool _damagedPlayerThisFrame = false;
    private bool _triedToAttackedPlayer = false;

    private Vector2 _target;
    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";
    private const string ANIM_ATTACK_1 = "SkeletonAttack1";
    
    public void AddSpeed(float speed)
    {
        _speed += speed;
    }

    bool CanUpdate()
    {
        return _playerReference && !_isDead;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        _target = Vector2.zero;
        _isDead = true;
        _animator.SetBool(ANIM_BOOL_DEAD, true);

        AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo(0);
        const int steps = 100;
        float stepTime = animInfo.length / steps;
        Color color = _spriteRenderer.color;

        for (int i = 0; i < steps; i++)
        {
            color.a = 1f - (i / (float)steps);
            _spriteRenderer.color = color;
            yield return new WaitForSeconds(stepTime);
        }

        Destroy(gameObject);
    }

    void Awake()
    {
        _playerReference = FindAnyObjectByType<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // I would like this simplify this attack logic
    // basically I want to say:
    // play attack animation 
    // _isAttacking = true 
    // if close enough apply damage just once!
    void Update()
    {
        if (!CanUpdate()) 
        {
            return; 
        }

        _spriteRenderer.flipX = _playerReference.transform.position.x < transform.position.x;
        AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo(0);

        bool isAttacking = animInfo.IsName(ANIM_ATTACK_1);
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        bool isCloseEnoughToAttackPlayer = distanceToPlayer <= _attackRadius;
        if (!isAttacking && !_triedToAttackedPlayer && isCloseEnoughToAttackPlayer)
        {
            _speed = 0f;
            _animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
            _triedToAttackedPlayer = true;
        }
        else if (!isAttacking)
        {
            _speed = 2f;
            _damagedPlayerThisFrame = false;
        }

        bool shouldDamagePlayer = (animInfo.normalizedTime >= 0.5f) && (distanceToPlayer <= _attackRadius * 1.1f);
        if (isAttacking && !_damagedPlayerThisFrame && shouldDamagePlayer)
        {
            _playerReference.ApplyDamage(1);
            _damagedPlayerThisFrame = true;
            _triedToAttackedPlayer = false;
        }

        _target = (_playerReference.transform.position - transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanUpdate())
        {
            return;
        }
        
        FireballController fireball = collision.GetComponent<FireballController>();
        if (fireball != null)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    void FixedUpdate()
    {
        if (!CanUpdate())
        {
            _target = Vector2.zero;
            _speed = 0f;
            _rb.linearVelocity = Vector2.zero;
            return; 
        }

        _rb.linearVelocity = _target * _speed;
    }
}
