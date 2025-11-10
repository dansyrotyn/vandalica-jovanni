using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering;

public class WizardEntity : EntityPlayer
{
    [SerializeField] private GameObject _UIHeartGrid;
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Collider2D _leftHurtBox;
    [SerializeField] private Collider2D _rightHurtBox;
    
    private const string ANIM_TRIGGER_HURT = "Hurt";
    private const string ANIM_TRIGGER_ATTACK = "Attack";
    private const string ANIM_DEATH = "WizardDeathAnim";

    [SerializeField] private float attackCooldownTime = 1.0f;
    [SerializeField] private float attackCooldown = 0.0f;

    void Start()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            Instantiate(_heartPrefab, _UIHeartGrid.transform);
        }

        _type = EntityPlayerType.WIZARD;
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

        StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        _visual.SpriteRenderer.color = Color.black;
        yield return new WaitForSeconds(0.05f);
        _visual.SpriteRenderer.color = Color.white;
    }

    private void HandleSpriteFlip()
    {
        if (_rb.linearVelocity.x != 0)
        {
            _visual.SpriteRenderer.flipX = _rb.linearVelocity.x < 0;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EntityEnemy enemy = collision.GetComponent<EntityEnemy>();
        if (enemy != null)
        {
            enemy.Damage(1);
            EnemyKillCount += 1;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        HandleSpriteFlip();

        if (attackCooldown == 0.0f)
        {
            _visual.Animator.SetTrigger(ANIM_TRIGGER_ATTACK);
            attackCooldown = attackCooldownTime;
        }
        else
        {
            attackCooldown = Math.Max(attackCooldown - Time.deltaTime, 0.0f);
        }

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

    private IEnumerator TriggerHurtBox(Collider2D box)
    {
        AnimatorStateInfo info = _visual.Animator.GetCurrentAnimatorStateInfo(0);
        box.gameObject.SetActive(true);
        Color color = _visual.SpriteRenderer.color;
        _visual.SpriteRenderer.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(info.length / 4.0f); // this is kind of arbitrary I should figure out something better...
        _visual.SpriteRenderer.color = color;
        box.gameObject.SetActive(false);
    }

    private void UnityAnimationEvent_Attack()
    {
        if (_visual.SpriteRenderer.flipX)
        {
            StartCoroutine(TriggerHurtBox(_leftHurtBox));
        }
        else
        {
            StartCoroutine(TriggerHurtBox(_rightHurtBox));
        }
    }
}
