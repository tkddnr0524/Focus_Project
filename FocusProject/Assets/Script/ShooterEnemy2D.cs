using MasterStylizedProjectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy2D : MonoBehaviour
{
    public BulletDatas datas;                 // 기존 BulletDatas 재사용
    public int index = 0;
    public Transform firePoint;
    public float fireInterval = 2f;

    public float Speed = 15f;
    public bool IsTargeting = false;
    public float RotSpeed = 0;

    float fireTimer;
    EffectsGroup CurEffect => datas.Effects[index];

    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 dir = GetPlayerDir();
        if (CurEffect.StartParticles != null)
        {
            var startFX = Instantiate(CurEffect.StartParticles, firePoint.position, Quaternion.identity);
            startFX.transform.right = dir;
            Destroy(startFX.gameObject, 1.5f); // 적절한 수명 정리
        }

        if (CurEffect.BulletParticles != null)
        {
            var obj = Instantiate(CurEffect.BulletParticles, firePoint.position, Quaternion.identity);
            obj.transform.right = dir;
            // 자식 오브젝트들을 Y축으로 90도 회전
            foreach (Transform child in obj.transform)
            {
                child.localRotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            obj.gameObject.layer = LayerMask.NameToLayer("Projectile");


            var bullet = obj.gameObject.AddComponent<Projectile2D>();
            bullet.Initialize(Speed, IsTargeting, RotSpeed, CurEffect.HitParticles);

            obj.gameObject.AddComponent<FocusSlowManager>();

            // 필요 시 sortingLayer 적용
            var rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.sortingLayerName = "Projectile";
                rend.sortingOrder = 5;
            }
        }
    }

    Vector2 GetPlayerDir()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return Vector2.right;
        return ((Vector2)player.transform.position - (Vector2)firePoint.position).normalized;
    }
}



