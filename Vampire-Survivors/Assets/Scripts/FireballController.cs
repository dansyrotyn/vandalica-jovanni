using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FireballController : MonoBehaviour
{
    private float angle;
    [SerializeField] private float speed = 4.0f;
    [SerializeField] private float radius = 0.20f;

    void Update()
    {
        angle += this.speed * Time.deltaTime;

        Vector2 position = Vector2.zero;
        position.x = this.radius * Mathf.Cos(angle);
        position.y = this.radius * Mathf.Sin(angle);

        this.transform.localPosition = position;
    }
}
