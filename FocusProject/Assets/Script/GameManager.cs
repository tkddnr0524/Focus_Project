using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;         // �÷��̾� ������ ����
    public Transform respawnPoint;          // ��Ȱ ��ġ ����
    public GameObject gameOverPanel;

    public GameObject pausePanel; // ���� UI �г�
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

        // �ʿ��ϸ� ���� �Ͻ�����, UI ǥ�� �� ó��
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
    /// �� ��ü�� ���ε��Ͽ� ������ ������ �ʱ�ȭ
    /// </summary>
    private void RestartGame()
    {
        Time.timeScale = 1f; // �ð� �ٽ� �帣��
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // ���� �� �ٽ� �ε�
    }

    /// <summary>
    /// ������ �Ͻ������ϰų� �簳
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // ��Ŀ�� ���� ����
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var focus = player.GetComponent<FocusSkillController>();
                if (focus != null && focus.isFocusActive)
                    focus.ForceDeactivate(); 
            }

            // 2) ���� �Ͻ�����
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            isPaused = true;
        }
        else
        {
            // ���� ����
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
