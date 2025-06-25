using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageManager : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab; // �ܻ�� ������ (SpriteRenderer�� �ʿ�)
    [SerializeField] private float spawnInterval = 0.1f;  // �ܻ� ���� ����
    [SerializeField] private float lifeTime = 0.5f;       // �ܻ� ���� �ð�

    private float timer = 0f;
    private SpriteRenderer playerSR;                      // �÷��̾� ��������Ʈ������ ����

    private void Start()
    {
        // �÷��̾� �ڽ�(�� ��ũ��Ʈ�� ���� ������Ʈ)�� SpriteRenderer ��������
        playerSR = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnAfterImage();
        }
    }

    private void SpawnAfterImage()
    {
        if (playerSR == null || afterImagePrefab == null) return;

        // �ܻ� ������Ʈ ����
        GameObject img = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        SpriteRenderer sr = img.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // �߰��ϴ� �ڵ� �Դϴ�
        sr.sprite = playerSR.sprite;              // ���� �÷��̾� ��������Ʈ ����
        sr.flipX = playerSR.flipX;               // �¿� ���� ���µ� ����
        sr.color = new Color(1f, 1f, 1f, 0.8f);   // �ʱ� ���� ����

        StartCoroutine(FadeAndDestroy(img, sr));
    }

    private IEnumerator FadeAndDestroy(GameObject obj, SpriteRenderer sr)
    {
        float elapsed = 0f;
        Color orig = sr.color;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifeTime;
            sr.color = new Color(orig.r, orig.g, orig.b, Mathf.Lerp(orig.a, 0f, t));
            yield return null;
        }

        Destroy(obj);
    }
}
