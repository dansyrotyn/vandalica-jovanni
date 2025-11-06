using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float speed;
    private Vector2 moveDirection;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        this.moveDirection = new Vector2(inputX, inputY).normalized;
        spriteRenderer.flipX = (moveDirection.x < 0) || (moveDirection.x == 0 && spriteRenderer.flipX);
    }

    void FixedUpdate()
    {
        this.rb.linearVelocity = this.moveDirection * this.speed;
    }
}
