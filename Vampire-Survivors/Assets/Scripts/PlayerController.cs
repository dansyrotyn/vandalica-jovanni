using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    private Vector2 _moveDirection;

    private const string ANIM_TRIGGER_HURT = "Hurt";

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        _animator.SetTrigger(ANIM_TRIGGER_HURT);
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(inputX, inputY).normalized;
    }

    private void HandleSpriteFlip()
    {
        if (_moveDirection.x != 0)
        {
            _spriteRenderer.flipX = _moveDirection.x < 0;
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        HandleInput();
        HandleSpriteFlip();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _moveDirection * speed;
    }
}
