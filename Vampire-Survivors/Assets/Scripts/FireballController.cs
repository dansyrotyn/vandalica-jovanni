using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FireballController : MonoBehaviour
{
    private float _angle;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _radius = 0.20f;

    void Update()
    {
        _angle += _speed * Time.deltaTime;

        Vector2 position = Vector2.zero;
        position.x = _radius * Mathf.Cos(_angle);
        position.y = _radius * Mathf.Sin(_angle);

        transform.localPosition = position;
    }
}
