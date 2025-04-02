using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int hp = 100;
    public Text hpText;
    public Text timerText;

    public GameObject gameStartPanel; 
    public Button startButton;
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button quitButton;
    public GameObject gameSuccessPanel;

    public float gameTime = 90f;
    private bool success = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Time.timeScale = 0;
        if (gameStartPanel != null)
        {
            if (PlayerPrefs.GetInt("Restarted", 0) == 1)
            {
                PlayerPrefs.SetInt("Restarted", 0);
                StartGame();
            }
            else
            {
                gameStartPanel.SetActive(true);
            }
        }
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    private void Update()
    {
        if (!success)
        {
            gameTime -= Time.deltaTime;
            UpdateTimerUI();

            if (gameTime <= 0)
            {
                gameTime = 0;
                GameSuccess();
            }
        }
    }
    public void StartGame()
    {
        Time.timeScale = 1;
        if (gameStartPanel != null)
        {
            gameStartPanel.SetActive(false);
        }
    }
    public void AddHp(int amount)
    {
        hp += amount;
        hp = Mathf.Clamp(hp, 0, 100);
        UpdateHpUI();

        if (hp <= 0)
        {
            GameOver();
        }
    }
    private void UpdateHpUI()
    {
        if (hpText != null)
        {
            hpText.text = "HP : " + hp;
        }
    }
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60);
            int seconds = Mathf.FloorToInt(gameTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    private void GameOver()
    {
        if (success) return;

        Time.timeScale = 0;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    private void GameSuccess()
    {
        if (success) return;
        success = true;

        Time.timeScale = 0;
        if (gameSuccessPanel != null)
        {
            gameSuccessPanel.SetActive(true);
        }
    }
    public void RestartGame()
    {
        PlayerPrefs.SetInt("Restarted", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
