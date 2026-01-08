using Mirror;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class KnightEntity : EntityPlayer
{
    [Header("Knight Entity Info")]
    [SerializeField] private TMP_Text _UIHeart;
    
    private const string ANIM_TRIGGER_HURT = "Hurt";
    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_DEATH = "KnightDeathAnim";

    protected override void Start()
    {
        base.Start();
        _UIHeart.text= _health.ToString();

        _type = EntityPlayerType.KNIGHT;
    }

    [ServerCallback]
    public override void GetDamage(int damage)
    {
        _health -= damage;
        RpcOnDamage(_health);
    }

    [ClientRpc]
    void RpcOnDamage(int currentHp)
    {
        _visual.Animator.SetTrigger(ANIM_TRIGGER_HURT);
        _UIHeart.text = currentHp.ToString();
    }

    private void HandleSpriteFlip()
    {
        if (_rb.linearVelocity.x != 0)
        {
            _visual.SpriteRenderer.flipX = _rb.linearVelocity.x < 0;
        }
    }

    [ServerCallback]
    private void Update()
    {
        if (_isDead) return;

        HandleSpriteFlip();
        if (_health <= 0)
        {
            _isDead = true;
            RpcDeath();
        }
    }

    [ClientRpc]
    void RpcDeath()
    {
        _visual.FadeOutDeathTask(ANIM_DEATH, false).ContinueWith(_ =>
        {
            GameManager.Instance.PlayerList.Remove(this);
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
}
