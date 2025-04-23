using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int hp = 100;
    public int score = 0;           // ���� ���� ���� �� ��
    public int destroyedMeteo = 0;  // � �ı� Ƚ��
    public bool success = false;    // ���� ���� ����
    public float gameTime = 90f;    // ���� �÷��� 90�ʷ� ����

    public List<GameResult> gameResults = new List<GameResult>();

    public Text hpText;
    public Text timerText;

    public EyesDataManager eyesDataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ���� ����
        }
        else Destroy(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayScene")
        {
            hpText = GameObject.Find("HpText")?.GetComponent<Text>();
            timerText = GameObject.Find("TimerText")?.GetComponent<Text>();

            UpdateHpUI();
            UpdateTimerUI();
        }
    }
    private void Start()
    {
        UpdateHpUI();
    }
    private void Update()
    {
        if (success) return;

        gameTime -= Time.deltaTime;
        UpdateTimerUI();

        if (gameTime <= 0)
        {
            GameSuccess();
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
            hpText.text = "HP : " + hp;
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
    private void GameSuccess()
    {
        success = true;
        Time.timeScale = 0;
        SaveGameResult();
        eyesDataManager.SaveEyesData();
        SceneManager.LoadScene("GameSuccessScene");
    }
    private void GameOver()
    {
        Time.timeScale = 0;
        SaveGameResult();
        eyesDataManager.SaveEyesData();
        SceneManager.LoadScene("GameOverScene");
    }
    private void SaveGameResult()
    {
        int index = gameResults.Count + 1; // �� ��° �÷�������
        var result = new GameResult(index, hp, success, score, destroyedMeteo, 90f - gameTime);
        gameResults.Add(result);
    }
    public void ResetGameData()
    {
        hp = 100;
        score = 0;
        destroyedMeteo = 0;
        success = false;
        gameTime = 90f;
    }
}