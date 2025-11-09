using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SkeletonController : EntityEnemy
{
    private EntityPlayer _playerReference;
    private FollowGameObject _follow;

    [SerializeField] private float _attackRadius = 1f;
    private bool _attackingPlayer = false;

    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";

    private const string ANIM_ATTACK_1 = "SkeletonAttack1";
     private const string ANIM_DEATH = "SkeletonDeathAnim";

    private bool CanUpdate() => (_playerReference != null) && !_isDead;

    private EntityPlayer GetClosestPlayer()
    {
        float minimumDistance = float.MaxValue;
        EntityPlayer closestPlayer = null;
        foreach (EntityPlayer player in GameManager.Instance.PlayerList)
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
            return closestPlayer.GetComponent<EntityPlayer>();
        }

        return null;
    }

    private void AttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        if (distanceToPlayer <= _attackRadius)
        {
            _playerReference.Damage(1);
        }
    }

    private void EndAttackAnimation()
    {
        _attackingPlayer = false;
    }

    private void Start()
    {
        _follow = GetComponent<FollowGameObject>();
    }

    void Update()
    {
        _playerReference = GetClosestPlayer();
        if (!CanUpdate())
        {
            return;
        }

        _follow.SetTarget(_playerReference.gameObject);
        _visual.FaceTarget(_playerReference.transform);

        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        bool isCloseEnoughToAttackPlayer = distanceToPlayer <= _attackRadius;
        if (!_attackingPlayer && isCloseEnoughToAttackPlayer)
        {
            _attackingPlayer = true;
            _visual.Animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
        }
    }
    
    public override void Damage(int dmg) {}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanUpdate())
        {
            return;
        }
        
        FireballController fireball = collision.GetComponent<FireballController>();
        if (fireball != null)
        {
            _isDead = true;
            _visual.FadeOutDeathTask(ANIM_DEATH, true).ContinueWith(_ => 
                {
                    GameManager.Instance.EnemyList.Remove(this);
                    Destroy(this.gameObject);
                }, 

                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }
    }
}
