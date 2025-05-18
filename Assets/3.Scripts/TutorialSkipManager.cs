using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro 사용 시

[System.Serializable]
public class SceneTutorialOption
{
    public string sceneName;
    public TextAsset messageTextAsset;
    // public TutorialSpawner tutorialSpawner; // 해당 씬에서 사용할 스포너 오브젝트 (삭제)
}

public class TutorialSkipManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI messageText; // 또는 public Text messageText;
    public Button okButton;

    public List<SceneTutorialOption> sceneTutorialOptions; // Inspector에서 씬별 옵션 할당

    private int step = 0;
    private string[] messages;
    // private TutorialSpawner tutorialSpawner; (삭제)

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        TextAsset selectedTextAsset = null;
        foreach (var option in sceneTutorialOptions)
        {
            if (option.sceneName == currentScene)
            {
                selectedTextAsset = option.messageTextAsset;
                // tutorialSpawner = option.tutorialSpawner; (삭제)
                break;
            }
        }
        if (selectedTextAsset != null)
        {
            // 줄바꿈 2개(빈 줄)로 메시지 구분
            messages = selectedTextAsset.text.Split(new string[] { "\n\n", "\r\n\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = messages[i].Trim();
            }
        }
        else
        {
            // 기본 메시지
            messages = new string[] {
                "우리의 목표는 지구를 지키기 위해\n날아오는 운석을 파괴하는 거에요!",
                "날아오는 운석을 3초간 응시하면\n운석을 부술 수 있어요!",
                "자, 그럼 시작해볼까요? 지구를 지켜주세요!"
            };
        }
        Time.timeScale = 0f;
        okButton.onClick.AddListener(OnOkClicked);
        ShowStep();
    }

    void ShowStep()
    {
        tutorialPanel.SetActive(true);
        if (messages == null || messages.Length == 0)
        {
            messages = new string[] { };
        }
        if (step < messages.Length)
        {
            messageText.text = messages[step];
            okButton.gameObject.SetActive(true);
            // dotSpawner 관련 코드 삭제
            // 지정된 메시지 인덱스에서 스포너 동작
            // if (step == spawnMessageIndex && tutorialSpawner != null)
            // {
            //     tutorialSpawner.SpawnMeteor();
            // }
        }
        else
        {
            // 튜토리얼 종료, 게임 재개
            Time.timeScale = 1f;
            tutorialPanel.SetActive(false);
        }
    }

    void OnOkClicked()
    {
        step++;
        ShowStep();
    }

    public void OnMeteorDestroyed()
    {
        // 운석 파괴 시 다음 단계로 진행
        step++;
        ShowStep();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
