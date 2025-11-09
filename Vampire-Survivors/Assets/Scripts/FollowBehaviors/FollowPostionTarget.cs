using UnityEngine;

public class FollowPositionTarget : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _distanceOffsetFromTarget = 0.05f;
    [SerializeField] private Vector3 _target = Vector3.zero;

    private Rigidbody2D _rb;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    public bool CloseToTarget()
    {
        float distanceFromTarget = Vector3.Distance(_target, this.transform.position);
        return distanceFromTarget <= _distanceOffsetFromTarget;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (_target == null)
        {
            _rb.linearVelocity = Vector3.zero; 
            return;
        }

        if (CloseToTarget())
        {
            _rb.linearVelocity = Vector3.zero;
        }
        else
        {
            _rb.linearVelocity = (_target - this.transform.position).normalized * _speed;
        }
    }
}