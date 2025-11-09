using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool shouldFreezePlayerController = false;

    private Vector3 _moveDirection;

    void FreezePlayerController()
    {
        shouldFreezePlayerController = true;
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(inputX, inputY).normalized;

        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.IsPaused())
        {
            GameManager.Instance.PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsPaused())
        {
            GameManager.Instance.ResumeGame();
        }
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        GameManager.Instance.EventControllablePlayerIsDead.AddListener(FreezePlayerController);
    }

    private void Update()
    {
        if (shouldFreezePlayerController) return;

        HandleInput();
    }

    private void FixedUpdate()
    {
        if (shouldFreezePlayerController)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        _rigidbody.linearVelocity = _moveDirection * _speed;
    }
}
