using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class FireballController : MonoBehaviour
{
    private float _angle;
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _radius = 0.20f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        EntityEnemy enemy = collision.GetComponent<EntityEnemy>();
        if (enemy != null)
        {
            enemy.Damage(1);

            Transform parent = this.gameObject.transform.parent;
            EntityPlayer player = parent.gameObject.GetComponent<EntityPlayer>();
            player.EnemyKillCount += 1;


            // NOTE(Jovanni):
            // because I know that this will kill anything I can just say
            // that if you damage someone you also killed them
            // but later this would have to be more robust.
        }
    }

    void Update()
    {
        _angle += _speed * Time.deltaTime;

        Vector2 position = Vector2.zero;
        position.x = _radius * Mathf.Cos(_angle);
        position.y = _radius * Mathf.Sin(_angle);

        transform.localPosition = position;
    }
}
