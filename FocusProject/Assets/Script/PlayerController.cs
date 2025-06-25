using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool isDead = false;
    private Animator anim;


    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("TriggerAppear");
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        // 사라짐 애니메이션 트리거
        anim.SetTrigger("TriggerDisappear");


        // Destroy는 애니메이션 끝날 때까지 대기
        StartCoroutine(WaitAndDestroy());

        GameManager.Instance.OnPlayerDeath();
    }

    private IEnumerator WaitAndDestroy()
    {
        // 애니메이션 길이에 맞게 시간 조절 (예: 1초)
        yield return new WaitForSeconds(0.1f); // 사라짐 애니 길이만큼
        Destroy(gameObject);
    }
}
