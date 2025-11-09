using System.Threading.Tasks;
using UnityEngine;

public class HeroEntity : EntityPlayer
{
    private const string ANIM_TRIGGER_HURT = "Hurt";
    private const string ANIM_BOOL_DEAD = "Dead";

    private const string ANIM_DEATH = "KnightDeathAnim";

    public override void Damage(int damage)
    {
        _health -= damage;
        _visual.Animator.SetTrigger(ANIM_TRIGGER_HURT);
    }

    private void HandleSpriteFlip()
    {
        if (_rb.linearVelocity.x != 0)
        {
            _visual.SpriteRenderer.flipX = _rb.linearVelocity.x < 0;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        HandleSpriteFlip();
        if (_health <= 0)
        {
            // could play like a death animation here
            _isDead = true;
            _visual.FadeOutDeathTask(ANIM_DEATH, false).ContinueWith(_ => 
                {
                    GameManager.Instance.PlayerList.Remove(this);
                    Destroy(this.gameObject);
                }, 

                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }
    }
}
