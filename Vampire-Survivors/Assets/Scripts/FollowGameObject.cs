using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distanceOffsetFromTarget; 
    [SerializeField] private GameObject _target;

    private Rigidbody2D _rb;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public bool CloseToTarget()
    {
        float distanceFromTarget = Vector3.Distance(_target.transform.position, this.transform.position);
        return distanceFromTarget <= _distanceOffsetFromTarget;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
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
            _rb.linearVelocity = (_target.transform.position - this.transform.position).normalized * _speed;
        }
    }
}