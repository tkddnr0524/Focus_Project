using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;         // 플레이어 프리팹 연결
    public Transform respawnPoint;          // 부활 위치 지정

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnPlayerDeath()
    {
        isGameOver = true;

        // 필요하면 게임 일시정지, UI 표시 등 처리
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
            Debug.LogError("playerPrefab 또는 respawnPoint가 할당되지 않았습니다.");
            return;
        }

        Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
    }
}
