using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private FollowPositionTarget _follow;
    private Vector3 _target;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _follow = GetComponent<FollowPositionTarget>();
    }

    private Vector3 GetNewRandomTargetPosition()
    {
        List<Vector3> area = GameManager.Instance.GetPlayableArea();
        int index = Random.Range(0, area.Count);
        
        return area[index];
    }

    private void Update()
    {
        if ((_target == Vector3.zero) || _follow.CloseToTarget())
        {
            _target = GetNewRandomTargetPosition();
            _follow.SetTarget(_target);
        }
    }
}
