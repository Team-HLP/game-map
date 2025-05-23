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

[System.Serializable]
public class TutorialPrefabOption
{
    public int messageIndex;      // 몇 번째 메시지에 프리팹을 생성할지
    public GameObject prefab;     // 생성할 프리팹
}

public class TutorialSkipManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI messageText; // 또는 public Text messageText;
    public Button okButton;

    public List<SceneTutorialOption> sceneTutorialOptions; // Inspector에서 씬별 옵션 할당
    public List<TutorialPrefabOption> tutorialPrefabOptions; // 인스펙터에서 할당
    public Transform objectSpawner; // 프리팹을 생성할 부모 오브젝트(빈 오브젝트)

    private int step = 0;
    private string[] messages;
    private GameObject spawnedObject; // 현재 생성된 프리팹
    private int currentPrefabIndex = -1; // 현재 생성된 프리팹의 인덱스
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
            // 프리팹 관리
            var prefabOption = tutorialPrefabOptions.Find(opt => opt.messageIndex == step);
            if (prefabOption != null && prefabOption.prefab != null)
            {
                // 이전 프리팹이 있으면 삭제
                if (spawnedObject != null)
                {
                    Destroy(spawnedObject);
                    Debug.Log("튜토리얼 프리팹이 삭제되었습니다.");
                }
                spawnedObject = Instantiate(prefabOption.prefab, objectSpawner.position, objectSpawner.rotation, objectSpawner);
                currentPrefabIndex = step;
                Debug.Log("튜토리얼 프리팹이 생성되었습니다.");
            }
            else
            {
                // 현재 프리팹이 있고, 인덱스가 넘어가면 삭제
                if (spawnedObject != null)
                {
                    Destroy(spawnedObject);
                    spawnedObject = null;
                    currentPrefabIndex = -1;
                    Debug.Log("튜토리얼 프리팹이 삭제되었습니다.");
                }
            }
        }
        else
        {
            // 튜토리얼 종료, 게임 재개
            Time.timeScale = 1f;
            tutorialPanel.SetActive(false);
            // 튜토리얼 종료 시 프리팹 삭제
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
                currentPrefabIndex = -1;
                Debug.Log("튜토리얼 프리팹이 삭제되었습니다.");
            }
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
