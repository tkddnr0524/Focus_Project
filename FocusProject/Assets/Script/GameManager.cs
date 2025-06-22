using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;         // �÷��̾� ������ ����
    public Transform respawnPoint;          // ��Ȱ ��ġ ����

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnPlayerDeath()
    {
        isGameOver = true;

        // �ʿ��ϸ� ���� �Ͻ�����, UI ǥ�� �� ó��
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (isGameOver && Input.anyKeyDown)
        {
            isGameOver = false;
            Time.timeScale = 1f;

            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || respawnPoint == null)
        {
            Debug.LogError("playerPrefab �Ǵ� respawnPoint�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
    }
}
