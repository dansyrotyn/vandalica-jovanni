using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerController playerReference;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float speed;
    [SerializeField] private bool isDead;

    private Vector3 target;

    // Attack cooldown thing and attack cone
    [SerializeField] private float attackRadius;


    private readonly string ANIM_BOOL_DEAD = "Dead";

    private readonly string ANIM_TRIGGER_ATTACK_1 = "Attack1";



    void Awake()
    {
        this.playerReference = Object.FindAnyObjectByType<PlayerController>();
        this.rb = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerReference == null) return;
        if (this.animator.GetBool(ANIM_BOOL_DEAD))
        {
            this.target = Vector3.zero;
        }

        this.spriteRenderer.flipX = playerReference.transform.position.x < this.transform.position.x;

        float distanceToPlayer = Vector3.Distance(this.playerReference.transform.position, this.transform.position);
        bool inAttackingAnimation = this.animator.GetCurrentAnimatorStateInfo(0).IsName(ANIM_TRIGGER_ATTACK_1);
        bool canAttack = (distanceToPlayer < this.attackRadius) && !inAttackingAnimation;
        if (canAttack)
        {
            this.animator.SetTrigger(ANIM_TRIGGER_ATTACK_1);
            this.target = Vector3.zero;
        }
        else
        {
            this.target = (playerReference.transform.position - this.transform.position).normalized;
        }
    }

    // Some type of time until dead that can be interupted
    // also fade the opacity slowly with the timer. When they get resurcted then 
    // set opacity to 1.0 and set isDead to false!

    void FixedUpdate()
    {
        this.rb.linearVelocity = this.target * this.speed;
    }
}
