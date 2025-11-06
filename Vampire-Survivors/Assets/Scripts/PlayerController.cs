using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float movementSpeed;
    private Vector2 movementDirection;

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

        this.movementDirection = new Vector2(inputX, inputY).normalized;
        spriteRenderer.flipX = (movementDirection.x < 0) || (movementDirection.x == 0 && spriteRenderer.flipX);
    }

    void FixedUpdate()
    {
        this.rb.linearVelocity = new Vector2(
            this.movementDirection.x * this.movementSpeed,
            this.movementDirection.y * this.movementSpeed
        );
    }
}
