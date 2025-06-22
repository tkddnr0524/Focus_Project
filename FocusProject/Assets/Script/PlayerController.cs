using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        GameManager.Instance.OnPlayerDeath();

        Destroy(gameObject);  // 플레이어 파괴
    }
}
