using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GazeRaycaster gazeRaycaster;

    public int hp = 100;
    public int score = 0;           // 점수 설정 아직 안 함
    public int destroyedMeteo = 0;  // 운석 파괴 횟수
    public bool success = false;    // 게임 성공 여부
    public float gameTime = 20f;    // 게임 플레이 90초로 제한

    public Text hpText;
    public Text timerText;
    public Text scoreText;

    public EyesDataManager eyesDataManager;
    public EEGDataManager eegDataManager;

    private float sceneStartTime;

    private string eyeFilePath;
    private string eegFilePath;
    private string behaviorFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sceneStartTime = Time.time;
            eyeFilePath = Path.Combine(Application.persistentDataPath, "eye_data.json");
            eegFilePath = Path.Combine(Application.persistentDataPath, "eeg_data.json");
            behaviorFilePath = Path.Combine(Application.persistentDataPath, "behavior_data.json");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (scene.name == "MeteoriteScene")
        {
            hpText = GameObject.Find("HpText")?.GetComponent<Text>();
            timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
            scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();

            eyesDataManager = GameObject.Find("EyesDataManager")?.GetComponent<EyesDataManager>();
            eegDataManager = GameObject.Find("EEGDataManager")?.GetComponent<EEGDataManager>();
            gazeRaycaster = GameObject.Find("GazeRaycaster")?.GetComponent<GazeRaycaster>();
            
            eyesDataManager.ReMeasuring();
            eegDataManager.ReMeasuring();

            UpdateHpUI();
            UpdateTimerUI();
            UpdateScoreUI();
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
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

    private Coroutine hpColorCoroutine;
    private Color originalHpColor;
    public Color damageColor = Color.red;
    public Color healColor = Color.green;

    public void FlashHpColor(bool isHeal)
    {
        if (hpColorCoroutine != null)
            StopCoroutine(hpColorCoroutine);

        hpColorCoroutine = StartCoroutine(FlashHpColorCoroutine(isHeal));
    }

    private IEnumerator FlashHpColorCoroutine(bool isHeal)
    {
        if (hpText == null) yield break;

        if (originalHpColor == default)
            originalHpColor = hpText.color;

        hpText.color = isHeal ? healColor : damageColor;
        yield return new WaitForSeconds(1f);
        hpText.color = originalHpColor;
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

    private Coroutine scoreColorCoroutine;
    private Color originalScoreColor;
    public Color highlightColor = Color.yellow;

    private IEnumerator FlashScoreColor()
    {
        if (scoreText == null) yield break;

        originalScoreColor = scoreText.color;
        scoreText.color = highlightColor;

        yield return new WaitForSeconds(1f);
        scoreText.color = originalScoreColor;
    }

    private float lastDestroyTime = -999f;
    private float comboThreshold = 10f;

    public void AddScore()
    {
        int meteoDestroyScore = 100;
        int bonusScore = 0;

        float now = Time.time;

        if (now - lastDestroyTime <= comboThreshold)
        {
            bonusScore = 50;
        }

        lastDestroyTime = now;

        score += meteoDestroyScore + bonusScore;

        UpdateScoreUI();

        if (scoreColorCoroutine != null)
        {
            StopCoroutine(scoreColorCoroutine);
        }
        scoreColorCoroutine = StartCoroutine(FlashScoreColor());
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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + score;
        }
    }

    private void GameSuccess()
    {
        gazeRaycaster.SaveUserStatusToJson();
        success = true;
        eyesDataManager.SaveEyesData();
        eegDataManager.SaveEEGData();
        FlyingObject.SavePrefabSpawnCount();
        SaveGameResult();
        SceneManager.LoadScene("GameSuccessScene");
    }

    private void GameOver()
    {
        gazeRaycaster.SaveUserStatusToJson();
        eyesDataManager.SaveEyesData();
        eegDataManager.SaveEEGData();
        FlyingObject.SavePrefabSpawnCount();
        SaveGameResult();
        SceneManager.LoadScene("GameOverScene");
    }

    private void SaveGameResult()
    {
        string result = success ? "SUCCESS" : "FAIL";
        StartCoroutine(GameResultCoroutine(result, score, hp, destroyedMeteo));
    }

    public float getFrameTime()
    {
        return Time.time - sceneStartTime;
    }

    public void ResetGameData()
    {
        hp = 100;
        score = 0;
        destroyedMeteo = 0;
        success = false;
        gameTime = 20f;

        if (eyesDataManager != null)
        {
            eyesDataManager.ResetManager();
        }
        if (eegDataManager != null)
        {
            eegDataManager.ResetManager();
        }
    }

    IEnumerator GameResultCoroutine(string result, int score, int hp, int meteorite_broken_count)
    {
        int meteorite_prefab_count = PlayerPrefs.GetInt("meteorite_prefab_count");
        int fuel_prefab_count = PlayerPrefs.GetInt("fuel_prefab_count");

        string jsonBody = JsonUtility.ToJson(new GameResultRequest(meteorite_prefab_count, fuel_prefab_count, result, score, hp, meteorite_broken_count));
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        byte[] gmaeResult = Encoding.UTF8.GetBytes(jsonBody);
        byte[] eegBytes = File.ReadAllBytes(eegFilePath);
        byte[] eyeBytes = File.ReadAllBytes(eyeFilePath);
        byte[] behaviorBytes = File.ReadAllBytes(behaviorFilePath);

        formData.Add(new MultipartFormFileSection("request", gmaeResult, "request.json", "application/json"));
        formData.Add(new MultipartFormFileSection("eeg_data_file", eegBytes, "eeg_data.json", "application/json"));
        formData.Add(new MultipartFormFileSection("eye_data_file", eyeBytes, "eye_data.json", "application/json"));
        formData.Add(new MultipartFormFileSection("behavior_file", behaviorBytes, "behavior_data.json", "application/json"));

        UnityWebRequest request = UnityWebRequest.Post(Apiconfig.url + "/game/meteorite", formData);
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token", ""));

        yield return request.SendWebRequest();
    }

    public void ImmeditelyBioDataSave()
    {
        eyesDataManager.ImmeditelyEyePupilDataSave();
        eegDataManager.ImmeditelyEegDataSave();
    }
}