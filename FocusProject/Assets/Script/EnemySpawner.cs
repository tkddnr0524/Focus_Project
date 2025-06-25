using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("난이도별 프리팹 목록")]
    [SerializeField] private GameObject[] easyEnemies;
    [SerializeField] private GameObject[] normalEnemies;
    [SerializeField] private GameObject[] hardEnemies;
    [SerializeField] private GameObject[] hellEnemies;

    [Header("스폰 설정")]
    [SerializeField] private float initialSpawnInterval = 2f; // 초기 스폰 간격
    [SerializeField] private float spawnIntervalDecreasePerPhase = 0.1f; // 페이즈당 간격 감소량
    [SerializeField] private int phaseDuration = 30; // 1페이즈 지속 시간 (초)
    [SerializeField] private Vector2 mapMin = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 mapMax = new Vector2(10f, 5f);
    [SerializeField] private float spawnPadding = 1f; // 외곽까지 거리
    [SerializeField] private float minSpawnDistance = 1.5f; // 다른 적과 최소 거리

    private float timer;
    private float elapsedTime; // 전체 경과 시간
    private int currentPhase = 0; // 페이즈 카운트
    private float spawnInterval;

    private List<Vector2> activeSpawnPoints = new List<Vector2>();

    void Start()
    {
        spawnInterval = initialSpawnInterval; // 초기값 세팅
        timer = 0f;
        elapsedTime = 0f;
        currentPhase = 0;
        activeSpawnPoints.Clear(); // ← 필요하면 위치 중복 방지용으로
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        UpdatePhaseAndDifficulty();

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    // 페이즈 계산 및 스폰 간격 조정
    void UpdatePhaseAndDifficulty()
    {
        int newPhase = Mathf.FloorToInt(elapsedTime / phaseDuration);

        if (newPhase > currentPhase)
        {
            currentPhase = newPhase;
            spawnInterval = Mathf.Max(0.2f, initialSpawnInterval - spawnIntervalDecreasePerPhase * currentPhase);
            // 필요시 spawnInterval 최소값 설정
        }
    }

    void SpawnEnemy()
    {
        GameObject prefab = GetEnemyByPhase(currentPhase);
        if (prefab == null) return;

        Vector2 spawnPos = GetValidSpawnPosition();
        if (spawnPos == Vector2.zero) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        Destroy(enemy, 15f); // 15초 뒤 적 오브젝트 자동 삭제
        activeSpawnPoints.Add(spawnPos);
    }

    GameObject GetEnemyByPhase(int phase)
    {
        // 페이즈가 올라갈수록 난이도 혼합
        // 비율 예: phase 0 -> 100% easy, phase 1 -> 70% easy 30% normal, phase 2 -> 40% easy, 30% normal, 30% hard 등

        float roll = Random.value; // 0~1 랜덤

        if (phase == 0) // 0~29초: easy만
        {
            return easyEnemies[Random.Range(0, easyEnemies.Length)];
        }
        else if (phase == 1) // 30~59초: easy 70% normal 30%
        {
            if (roll < 0.7f)
                return easyEnemies[Random.Range(0, easyEnemies.Length)];
            else
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
        }
        else if (phase == 2) // 60~89초: easy 40%, normal 30%, hard 30%
        {
            if (roll < 0.4f)
                return easyEnemies[Random.Range(0, easyEnemies.Length)];
            else if (roll < 0.7f)
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
            else
                return hardEnemies[Random.Range(0, hardEnemies.Length)];
        }
        else if (phase == 3) // 90초 이상: normal 40%, hard 40%, hell 20%
        {
            if (roll < 0.4f)
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
            else if (roll < 0.8f)
                return hardEnemies[Random.Range(0, hardEnemies.Length)];
            else
                return hellEnemies[Random.Range(0, hellEnemies.Length)];
        }
        else // 4페이즈 이상(120초 이상) - 완전 혼합
        {
            int pool = Random.Range(0, 4);
            return pool switch
            {
                0 => easyEnemies[Random.Range(0, easyEnemies.Length)],
                1 => normalEnemies[Random.Range(0, normalEnemies.Length)],
                2 => hardEnemies[Random.Range(0, hardEnemies.Length)],
                _ => hellEnemies[Random.Range(0, hellEnemies.Length)],
            };
        }
    }

    Vector2 GetValidSpawnPosition()
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 pos = GetRandomEdgePosition();
            bool tooClose = false;

            foreach (Vector2 other in activeSpawnPoints)
            {
                if (Vector2.Distance(pos, other) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return pos;
        }

        return Vector2.zero; // 실패
    }

    Vector2 GetRandomEdgePosition()
    {
        float x, y;
        int side = Random.Range(0, 4); // 0=위, 1=아래, 2=왼쪽, 3=오른쪽

        switch (side)
        {
            case 0: // 위
                x = Random.Range(mapMin.x + spawnPadding, mapMax.x - spawnPadding);
                y = mapMax.y - spawnPadding;
                break;
            case 1: // 아래
                x = Random.Range(mapMin.x + spawnPadding, mapMax.x - spawnPadding);
                y = mapMin.y + spawnPadding;
                break;
            case 2: // 왼쪽
                x = mapMin.x + spawnPadding;
                y = Random.Range(mapMin.y + spawnPadding, mapMax.y - spawnPadding);
                break;
            case 3: // 오른쪽
                x = mapMax.x - spawnPadding;
                y = Random.Range(mapMin.y + spawnPadding, mapMax.y - spawnPadding);
                break;
            default:
                x = 0;
                y = 0;
                break;
        }

        return new Vector2(x, y);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 bottomLeft = new Vector3(mapMin.x, mapMin.y, 0);
        Vector3 topRight = new Vector3(mapMax.x, mapMax.y, 0);
        Vector3 center = (bottomLeft + topRight) / 2f;
        Vector3 size = topRight - bottomLeft;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}