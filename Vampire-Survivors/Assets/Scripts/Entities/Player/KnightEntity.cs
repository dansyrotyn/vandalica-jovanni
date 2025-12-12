using System.Threading.Tasks;
using UnityEngine;

public class KnightEntity : EntityPlayer
{
    [Header("Knight Entity Info")]
    [SerializeField] private GameObject _UIHeartGrid;
    [SerializeField] private GameObject _heartPrefab;

    private const string ANIM_TRIGGER_HURT = "Hurt";
    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_DEATH = "KnightDeathAnim";

    private Vector3 startPos;

    protected override void Start()
    {
        base.Start();
        var hearts = Mathf.Min(4, _maxHealth);
        for (int i = 0; i < hearts; i++)
        {
            Instantiate(_heartPrefab, _UIHeartGrid.transform);
        }

        _type = EntityPlayerType.KNIGHT;

        startPos = transform.position;
    }

    public override void Reborn()
    {
        base.Reborn();

        transform.position = startPos;

        _visual.Reborn();

        var hearts = Mathf.Min(4, _maxHealth) - _UIHeartGrid.transform.childCount;
        for (int i = 0; i < hearts; i++)
        {
            Instantiate(_heartPrefab, _UIHeartGrid.transform);
        }
        gameObject.SetActive(true);
    }

    public override void Damage(int damage)
    {
        if (IsLocal)
        {
            Health -= damage;
            _visual.Animator.SetTrigger(ANIM_TRIGGER_HURT);

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
        if (Health <= 0)
        {
            _isDead = true;
            _visual.FadeOutDeathTask(ANIM_DEATH, false).ContinueWith(_ =>
                {
                    //GameManager.Instance.PlayerList.Remove(this);
                    gameObject.SetActive(false);
                    OnDeath();
                },

                TaskScheduler.FromCurrentSynchronizationContext()
            );
        }
        else if (_UIHeartGrid.transform.childCount > Health)
        {
            Transform child = _UIHeartGrid.transform.GetChild(0);
            Destroy(child.gameObject);
        }
    }
}
