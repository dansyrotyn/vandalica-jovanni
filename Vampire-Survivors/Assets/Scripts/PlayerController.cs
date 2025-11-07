using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int health;

    [SerializeField] private float speed;
    private Vector2 moveDirection;

    private readonly string ANIM_TRIGGER_HURT = "Hurt";

    void Awake()
    {
        this.health = maxHealth;
        this.rb = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();
    }
    
    public void applyDamage(int dmg)
    {
        this.health -= dmg;
        this.animator.SetTrigger(ANIM_TRIGGER_HURT);
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        this.moveDirection = new Vector2(inputX, inputY).normalized;
        spriteRenderer.flipX = (moveDirection.x < 0) || (moveDirection.x == 0 && spriteRenderer.flipX);

        if (this.health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        this.rb.linearVelocity = this.moveDirection * this.speed;
    }
}
