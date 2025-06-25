using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile2D : MonoBehaviour, IFocusAffectable
{
    public float Speed;
    public bool IsTargeting; 
    public float RotSpeed;  
    public ParticleSystem HitEffect;

    private float currentSpeed;
    private float baseRotSpeed; // 원래 회전 속도 저장
    private Transform target;
    private Rigidbody2D rigidbody2D;



    void Start()
    {
        // Rigidbody2D 컴포넌트 추가 및 설정
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }
        rigidbody2D.isKinematic = true;      // 외부 물리 영향 안 받음
        rigidbody2D.simulated = true;        // 물리 시뮬레이션 활성화

        var collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, new Vector2(transform.forward.x, transform.forward.y) * 2f, Color.red, 2f);
    }

    void FixedUpdate()
    {
        Vector2 moveDir = new Vector2(transform.forward.x, transform.forward.y);

        if (IsTargeting && target != null)
        {
            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            Vector3 targetDir = new Vector3(toTarget.x, toTarget.y, 0f);
            if (targetDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.back); 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotSpeed * Time.fixedDeltaTime);
            }
            moveDir = new Vector2(transform.forward.x, transform.forward.y);
        }

        rigidbody2D.MovePosition(rigidbody2D.position + moveDir * currentSpeed * Time.fixedDeltaTime);
    }
    public void Initialize(float speed, bool isTargeting, float rotSpeed, ParticleSystem hitEffect)
    {
        Speed = speed;
        currentSpeed = speed;
        IsTargeting = isTargeting;
        RotSpeed = rotSpeed;
        baseRotSpeed = rotSpeed;
        HitEffect = hitEffect;

        if (IsTargeting)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    public void ApplyFocusSlow(float factor)
    {
        currentSpeed = Speed * factor;
        RotSpeed = baseRotSpeed * factor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (HitEffect != null)
                Instantiate(HitEffect, transform.position, Quaternion.identity);

            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
                player.Die();

            Destroy(gameObject);
        }
        else
        {
            // 플레이어 외 다른 콜라이더와 충돌 시에도 히트 이펙트 생성 후 파괴
            if (HitEffect != null)
                Instantiate(HitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
