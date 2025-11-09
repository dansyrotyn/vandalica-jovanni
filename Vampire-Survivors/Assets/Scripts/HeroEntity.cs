using UnityEngine;

public class HeroEntity : Entity
{
    private const string ANIM_TRIGGER_HURT = "Hurt";

    public void ApplyDamage(int damage)
    {
        _health -= damage;
        _animator.SetTrigger(ANIM_TRIGGER_HURT);
    }

    private void HandleSpriteFlip()
    {
        if (_rb.linearVelocity.x != 0)
        {
            _spriteRenderer.flipX = _rb.linearVelocity.x < 0;
        }
    }

    private void Update()
    {
        HandleSpriteFlip();

        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
