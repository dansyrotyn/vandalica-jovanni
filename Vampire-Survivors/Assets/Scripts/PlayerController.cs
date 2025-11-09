using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [Header("Movement Stats")]
    [SerializeField] private float _speed = 5f;

    private Vector3 _moveDirection;

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(inputX, inputY).normalized;

        if (Input.GetKeyDown(KeyCode.Escape) && !GameState.Instance.IsPaused())
        {
            GameState.Instance.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameState.Instance.IsPaused())
        {
            GameState.Instance.ResumeGame();
        }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = _moveDirection * _speed;
    }
}
