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
    public GameObject gameOverPanel;
    public GameObject gameStartPanel;
    public Button startButton;
    public Button restartButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 0; // ∞‘¿” Ω√¿€ ¿¸ ∏ÿ√„
        if (gameStartPanel != null)
        {
            gameStartPanel.SetActive(true);
        }
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
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
            hpText.text = "HP: " + hp;
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
