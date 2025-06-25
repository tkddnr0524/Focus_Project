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
        // ����� �ִϸ��̼� Ʈ����
        anim.SetTrigger("TriggerDisappear");


        // Destroy�� �ִϸ��̼� ���� ������ ���
        StartCoroutine(WaitAndDestroy());

        GameManager.Instance.OnPlayerDeath();
    }

    private IEnumerator WaitAndDestroy()
    {
        // �ִϸ��̼� ���̿� �°� �ð� ���� (��: 1��)
        yield return new WaitForSeconds(0.1f); // ����� �ִ� ���̸�ŭ
        Destroy(gameObject);
    }
}
