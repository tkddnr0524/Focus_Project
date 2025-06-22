using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;           // 프리팹 연결
    public float fireInterval = 2f;           // 발사 간격

    private float fireTimer;
    private Transform player;


    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return; // 플레이어가 아직 씬에 없으면 공격 안함
        }

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // 초기화 함수로 방향 전달
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(direction);
    }
}
