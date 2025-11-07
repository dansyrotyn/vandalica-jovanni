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

    private bool _shouldDie;
    private bool _isDead;
    private bool _hasDealtDamageThisFrame;

    private Vector2 _target;
    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";
    private const string ANIM_ATTACK_1 = "SkeletonAttack1";

    void Start()
    {
        _playerReference = FindAnyObjectByType<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private IEnumerator FadeOutAndDestroy()
    {
        _target = Vector2.zero;
        _isDead = true;
        _shouldDie = false;
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

    void Update()
    {
        if (_playerReference == null || _isDead)
        {
            _target = Vector2.zero;
            return;
        }

        if (_shouldDie)
        {
            StartCoroutine(FadeOutAndDestroy());
            return;
        }

        _spriteRenderer.flipX = _playerReference.transform.position.x < transform.position.x;

        AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        bool inAttack = animInfo.IsName(ANIM_ATTACK_1);
        bool canAttack = distanceToPlayer < _attackRadius && !inAttack;
        bool shouldDamage = inAttack && animInfo.normalizedTime >= 0.5f && distanceToPlayer <= _attackRadius * 1.1f;

        if (canAttack)
        {
            _animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
            _target = Vector2.zero;
            _hasDealtDamageThisFrame = false;
            return;
        }
        else
        {
            _target = (_playerReference.transform.position - transform.position).normalized;
        }

        if (shouldDamage && !_hasDealtDamageThisFrame)
        {
            _playerReference.ApplyDamage(1);
            _hasDealtDamageThisFrame = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerReference == null || _isDead)
        {
            _target = Vector2.zero;
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
        if (_playerReference == null || _isDead)
        {
            _target = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = _target * _speed;
    }
}
