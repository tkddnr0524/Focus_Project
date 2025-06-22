using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFocusAffectable
{
    void ApplyFocusSlow(float slowFactor);
}
public class Bullet : MonoBehaviour, IFocusAffectable
{
    public float baseSpeed = 10f;
    private float currentSpeed;
    private Vector2 direction;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void ApplyFocusSlow(float factor)
    {
        currentSpeed = baseSpeed * factor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.Die();
        }
    }
}
