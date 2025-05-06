using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectGameScene");
    }
    public void OnMeteoriteButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }

        Time.timeScale = 1;
        // 베이스 라인 씬에서 다음 씬으로 넘어가기 위해 저장
        PlayerPrefs.SetString("gameScene", "MeteoriteScene");
        PlayerPrefs.Save();
        SceneManager.LoadScene("EyeDataMeausreScene");
    }
}