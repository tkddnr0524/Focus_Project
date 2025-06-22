using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    public GameObject bulletPrefab;           // ������ ����
    public float fireInterval = 2f;           // �߻� ����

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
                return; // �÷��̾ ���� ���� ������ ���� ����
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

        // �ʱ�ȭ �Լ��� ���� ����
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(direction);
    }
}
