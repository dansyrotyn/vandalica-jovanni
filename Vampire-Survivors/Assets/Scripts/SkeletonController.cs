using System;
using System.Collections;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private HeroEntity _playerReference;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private FollowGameObject _follow;

    [SerializeField] private float _attackRadius = 1f;

    private bool _isDead = false;
    private bool _damagedPlayerThisFrame = false;
    private bool _triedToAttackedPlayer = false;

    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";
    private const string ANIM_ATTACK_1 = "SkeletonAttack1";

    private bool CanUpdate() => (_playerReference != null) && !_isDead;

    private IEnumerator FadeOutAndDestroy()
    {
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

        GameState.Instance.EnemyList.Remove(gameObject);
        Destroy(gameObject);
    }

    private HeroEntity GetClosestPlayer()
    {
        float minimumDistance = float.MaxValue;
        GameObject closestPlayer = null;
        foreach (GameObject player in GameState.Instance.PlayerList)
        {
            float dist = Vector3.Distance(this.transform.position, player.transform.position);
            if (dist < minimumDistance)
            {
                closestPlayer = player;
                minimumDistance = dist;
            }
        }

        if (closestPlayer != null)
        {
            return closestPlayer.GetComponent<HeroEntity>();
        }

        return null;
    }

    private void Start()
    {
        _follow = GetComponent<FollowGameObject>();
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
        // NOTE(Jovanni): probably horrible for perforance but I will fix this.
        _playerReference = GetClosestPlayer();
        if (!CanUpdate())
        {
            return; 
        }

        _follow.SetTarget(_playerReference.gameObject);
        _spriteRenderer.flipX = _playerReference.transform.position.x < transform.position.x;
        AnimatorStateInfo animInfo = _animator.GetCurrentAnimatorStateInfo(0);

        bool isAttacking = animInfo.IsName(ANIM_ATTACK_1);
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        bool isCloseEnoughToAttackPlayer = distanceToPlayer <= _attackRadius;
        if (!isAttacking && !_triedToAttackedPlayer && isCloseEnoughToAttackPlayer)
        {
            _animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
            _triedToAttackedPlayer = true;
        }
        else if (!isAttacking)
        {
            _damagedPlayerThisFrame = false;
        }

        bool shouldDamagePlayer = (animInfo.normalizedTime >= 0.5f) && (distanceToPlayer <= _attackRadius * 1.1f);
        if (isAttacking && !_damagedPlayerThisFrame && shouldDamagePlayer)
        {
            _playerReference.Damage(1);
            _damagedPlayerThisFrame = true;
            _triedToAttackedPlayer = false;
        }
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
}
