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
        Vector2 dir2D = GetPlayerDir();
        Vector3 dir3D = new Vector3(dir2D.x, dir2D.y, 0f);  // XY → XYZ

        if (CurEffect.StartParticles != null)
        {
            var startFX = Instantiate(
                CurEffect.StartParticles,
                firePoint.position,
                Quaternion.identity
            );
            startFX.transform.forward = dir3D.normalized;  // 추가하는 코드 입니다
            Destroy(startFX.gameObject, 1.5f);
        }

        if (CurEffect.BulletParticles != null)
        {
            var obj = Instantiate(
                CurEffect.BulletParticles,
                firePoint.position,
                Quaternion.identity
            );

            // 추가하는 코드 입니다: +Z(머리)가 dir3D 방향을 바라보도록 설정
            obj.transform.forward = dir3D.normalized;

            // 이전에 썼던 transform.right, Rotate 등 불필요하니 모두 제거

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



