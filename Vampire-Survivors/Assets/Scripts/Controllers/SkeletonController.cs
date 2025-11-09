using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class SkeletonController : EntityEnemy
{
    private EntityPlayer _playerReference;
    private FollowGameObject _follow;

    [SerializeField] private float _attackRadius = 1f;
    private bool _damagedPlayerThisFrame = false;
    private bool _triedToAttackedPlayer = false;

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

    private void Start()
    {
        _follow = GetComponent<FollowGameObject>();
    }

    // I would like this simplify this attack logic
    // basically I want to say:
    // play attack animation 
    // _isAttacking = true 
    // if close enough apply damage just once!

    // Make this animation events!
    void Update()
    {
        // NOTE(Jovanni): probably horrible for perforance but I will fix this.
        _playerReference = GetClosestPlayer();
        if (!CanUpdate())
        {
            return;
        }

        _follow.SetTarget(_playerReference.gameObject);
        _visual.FaceTarget(_playerReference.transform);
        AnimatorStateInfo animInfo = _visual.Animator.GetCurrentAnimatorStateInfo(0);

        bool isAttacking = animInfo.IsName(ANIM_ATTACK_1);
        float distanceToPlayer = Vector2.Distance(_playerReference.transform.position, transform.position);
        bool isCloseEnoughToAttackPlayer = distanceToPlayer <= _attackRadius;
        if (!isAttacking && !_triedToAttackedPlayer && isCloseEnoughToAttackPlayer)
        {
            _visual.Animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
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
