using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class KnightEntity : EntityPlayer
{
    [Header("Knight Entity Info")]
    [SerializeField] private GameObject _UIHeartGrid;
    [SerializeField] private GameObject _heartPrefab;
    
    private const string ANIM_TRIGGER_HURT = "Hurt";
    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_DEATH = "KnightDeathAnim";

    void Start()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            Instantiate(_heartPrefab, _UIHeartGrid.transform);
        }

        _type = EntityPlayerType.KNIGHT;
    }

    public override void Damage(int damage)
    {
        _health -= damage;
        _visual.Animator.SetTrigger(ANIM_TRIGGER_HURT);
        if (_UIHeartGrid.transform.childCount != 0)
        {
            Transform child = _UIHeartGrid.transform.GetChild(0);
            Destroy(child.gameObject);
        }
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
