using Mirror;
using System.Threading.Tasks;
using UnityEngine;

public class SkeletonEntity : EntityEnemy
{
    private EntityPlayer _playerReference;
    private FollowGameObject _follow;

    [Header("Skeleton Info")]
    [SerializeField] private float _attackRadius = 1f;
    
    private bool _attackingPlayer = false;
    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";
    private const string ANIM_ATTACK_1 = "SkeletonAttack1";
    private const string ANIM_DEATH = "SkeletonDeathAnim";

    [ServerCallback]
    public override void GetDamage(int damage)
    {
        _health -= damage;
    }

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

    private void Start()
    {
        _follow = GetComponent<FollowGameObject>();
    }

    [ServerCallback]

    private void Update()
    {
        if (_isDead) return;

        _playerReference = GetClosestPlayer();
        if (_playerReference == null)
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
            RpcAttack();
        }

        if (_health <= 0)
        {
            RpcDeath();
        }
    }

    [ClientRpc]
    void RpcAttack()
    {
        _visual.Animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
    }

    [ClientRpc]
    void RpcDeath()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;
        _visual.FadeOutDeathTask(ANIM_DEATH, true).ContinueWith(_ =>
        {
            GameManager.Instance.EnemyList.Remove(this);
            ServerDeath();
        },

                TaskScheduler.FromCurrentSynchronizationContext()
            );
    }

    [ServerCallback]
    void ServerDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void UnityAnimationEvent_TryAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        if (distanceToPlayer <= _attackRadius)
        {
            _playerReference.GetDamage(1);
        }
    }

    private void UnityAnimationEvent_EndAttackAnimation()
    {
        _attackingPlayer = false;
    }
}