using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;         // 플레이어 프리팹 연결
    public Transform respawnPoint;          // 부활 위치 지정
    public GameObject gameOverPanel;

    public GameObject pausePanel; // 퍼즈 UI 패널
    private bool isPaused = false;

    public bool IsPaused => isPaused;

    public bool IsGameOver => isGameOver;

    private bool isGameOver = false;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnPlayerDeath()
    {
        if (isGameOver) return;

        isGameOver = true;

        // 필요하면 게임 일시정지, UI 표시 등 처리
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void Update()
    {
        if (isGameOver && Input.anyKeyDown)
        {
            RestartGame();
        }

        if (!isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 씬 전체를 리로드하여 게임을 완전히 초기화
    /// </summary>
    private void RestartGame()
    {
        Time.timeScale = 1f; // 시간 다시 흐르게
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 현재 씬 다시 로드
    }

    /// <summary>
    /// 게임을 일시정지하거나 재개
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // 포커스 강제 해제
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var focus = player.GetComponent<FocusSkillController>();
                if (focus != null && focus.isFocusActive)
                    focus.ForceDeactivate(); 
            }

            // 2) 게임 일시정지
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            isPaused = true;
        }
        else
        {
            // 퍼즈 해제
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            isPaused = false;
        }

    }

    public void OnResumeButton()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
