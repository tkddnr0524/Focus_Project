using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageManager : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab; // 잔상용 프리팹 (SpriteRenderer만 필요)
    [SerializeField] private float spawnInterval = 0.1f;  // 잔상 생성 간격
    [SerializeField] private float lifeTime = 0.5f;       // 잔상 지속 시간

    private float timer = 0f;
    private SpriteRenderer playerSR;                      // 플레이어 스프라이트렌더러 참조

    private void Start()
    {
        // 플레이어 자신(이 스크립트가 붙은 오브젝트)의 SpriteRenderer 가져오기
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

        // 잔상 오브젝트 생성
        GameObject img = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        SpriteRenderer sr = img.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // 추가하는 코드 입니다
        sr.sprite = playerSR.sprite;              // 현재 플레이어 스프라이트 복사
        sr.flipX = playerSR.flipX;               // 좌우 반전 상태도 복사
        sr.color = new Color(1f, 1f, 1f, 0.8f);   // 초기 투명도 조절

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
