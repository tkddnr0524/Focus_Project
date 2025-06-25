using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("���̵��� ������ ���")]
    [SerializeField] private GameObject[] easyEnemies;
    [SerializeField] private GameObject[] normalEnemies;
    [SerializeField] private GameObject[] hardEnemies;
    [SerializeField] private GameObject[] hellEnemies;

    [Header("���� ����")]
    [SerializeField] private float initialSpawnInterval = 2f; // �ʱ� ���� ����
    [SerializeField] private float spawnIntervalDecreasePerPhase = 0.1f; // ������� ���� ���ҷ�
    [SerializeField] private int phaseDuration = 30; // 1������ ���� �ð� (��)
    [SerializeField] private Vector2 mapMin = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 mapMax = new Vector2(10f, 5f);
    [SerializeField] private float spawnPadding = 1f; // �ܰ����� �Ÿ�
    [SerializeField] private float minSpawnDistance = 1.5f; // �ٸ� ���� �ּ� �Ÿ�

    private float timer;
    private float elapsedTime; // ��ü ��� �ð�
    private int currentPhase = 0; // ������ ī��Ʈ
    private float spawnInterval;

    private List<Vector2> activeSpawnPoints = new List<Vector2>();

    void Start()
    {
        spawnInterval = initialSpawnInterval; // �ʱⰪ ����
        timer = 0f;
        elapsedTime = 0f;
        currentPhase = 0;
        activeSpawnPoints.Clear(); // �� �ʿ��ϸ� ��ġ �ߺ� ����������
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

    // ������ ��� �� ���� ���� ����
    void UpdatePhaseAndDifficulty()
    {
        int newPhase = Mathf.FloorToInt(elapsedTime / phaseDuration);

        if (newPhase > currentPhase)
        {
            currentPhase = newPhase;
            spawnInterval = Mathf.Max(0.2f, initialSpawnInterval - spawnIntervalDecreasePerPhase * currentPhase);
            // �ʿ�� spawnInterval �ּҰ� ����
        }
    }

    void SpawnEnemy()
    {
        GameObject prefab = GetEnemyByPhase(currentPhase);
        if (prefab == null) return;

        Vector2 spawnPos = GetValidSpawnPosition();
        if (spawnPos == Vector2.zero) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        Destroy(enemy, 15f); // 15�� �� �� ������Ʈ �ڵ� ����
        activeSpawnPoints.Add(spawnPos);
    }

    GameObject GetEnemyByPhase(int phase)
    {
        // ����� �ö󰥼��� ���̵� ȥ��
        // ���� ��: phase 0 -> 100% easy, phase 1 -> 70% easy 30% normal, phase 2 -> 40% easy, 30% normal, 30% hard ��

        float roll = Random.value; // 0~1 ����

        if (phase == 0) // 0~29��: easy��
        {
            return easyEnemies[Random.Range(0, easyEnemies.Length)];
        }
        else if (phase == 1) // 30~59��: easy 70% normal 30%
        {
            if (roll < 0.7f)
                return easyEnemies[Random.Range(0, easyEnemies.Length)];
            else
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
        }
        else if (phase == 2) // 60~89��: easy 40%, normal 30%, hard 30%
        {
            if (roll < 0.4f)
                return easyEnemies[Random.Range(0, easyEnemies.Length)];
            else if (roll < 0.7f)
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
            else
                return hardEnemies[Random.Range(0, hardEnemies.Length)];
        }
        else if (phase == 3) // 90�� �̻�: normal 40%, hard 40%, hell 20%
        {
            if (roll < 0.4f)
                return normalEnemies[Random.Range(0, normalEnemies.Length)];
            else if (roll < 0.8f)
                return hardEnemies[Random.Range(0, hardEnemies.Length)];
            else
                return hellEnemies[Random.Range(0, hellEnemies.Length)];
        }
        else // 4������ �̻�(120�� �̻�) - ���� ȥ��
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

        return Vector2.zero; // ����
    }

    Vector2 GetRandomEdgePosition()
    {
        float x, y;
        int side = Random.Range(0, 4); // 0=��, 1=�Ʒ�, 2=����, 3=������

        switch (side)
        {
            case 0: // ��
                x = Random.Range(mapMin.x + spawnPadding, mapMax.x - spawnPadding);
                y = mapMax.y - spawnPadding;
                break;
            case 1: // �Ʒ�
                x = Random.Range(mapMin.x + spawnPadding, mapMax.x - spawnPadding);
                y = mapMin.y + spawnPadding;
                break;
            case 2: // ����
                x = mapMin.x + spawnPadding;
                y = Random.Range(mapMin.y + spawnPadding, mapMax.y - spawnPadding);
                break;
            case 3: // ������
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