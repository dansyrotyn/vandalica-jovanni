using System.Collections;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController playerReference;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float speed = 2f;

    private bool shouldDie;
    private bool isDead;
    private bool hasDealtDamageThisFrame;

    private Vector2 target;

    private const string ANIM_BOOL_DEAD = "Dead";
    private const string ANIM_TRIGGER_ATTACK_1 = "Attack1";
    private const string ANIM_ATTACK_1 = "SkeletonAttack1";

    void Start()
    {
        playerReference = FindAnyObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private IEnumerator FadeOutAndDestroy()
    {
        target = Vector2.zero;
        isDead = true;
        shouldDie = false;
        animator.SetBool(ANIM_BOOL_DEAD, true);

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        const int steps = 100;
        float stepTime = animInfo.length / steps;
        Color color = spriteRenderer.color;

        for (int i = 0; i < steps; i++)
        {
            color.a = 1f - (i / (float)steps);
            spriteRenderer.color = color;
            yield return new WaitForSeconds(stepTime);
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (this.playerReference == null || this.isDead)
        {
            target = Vector2.zero;
            return;
        }

        if (shouldDie)
        {
            StartCoroutine(FadeOutAndDestroy());
            return;
        }

        spriteRenderer.flipX = playerReference.transform.position.x < transform.position.x;

        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float distanceToPlayer = Vector2.Distance(playerReference.transform.position, transform.position);
        bool inAttack = animInfo.IsName(ANIM_ATTACK_1);
        bool canAttack = distanceToPlayer < attackRadius && !inAttack;
        bool shouldDamage = inAttack && animInfo.normalizedTime >= 0.5f && distanceToPlayer <= attackRadius * 1.1f;

        if (canAttack)
        {
            animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
            target = Vector2.zero;
            hasDealtDamageThisFrame = false;
            return;
        }
        else
        {
            target = (playerReference.transform.position - transform.position).normalized;
        }

        if (shouldDamage && !hasDealtDamageThisFrame)
        {
            playerReference.applyDamage(1);
            hasDealtDamageThisFrame = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.playerReference == null || this.isDead)
        {
            target = Vector2.zero;
            return;
        }

        FireballController fireball = collision.GetComponent<FireballController>();
        if (fireball != null)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    void FixedUpdate()
    {
        if (this.playerReference == null || this.isDead)
        {
            target = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = target * speed;
    }
}
