using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonManager : MonoBehaviour
{
    public GameObject loginPopup;

    public void OnStartButtonClicked()
    {
        int isLoggedIn = PlayerPrefs.GetInt("loginSuccess", 0);

        if (isLoggedIn == 1)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("SelectGameScene");
        }
        else
        {
            loginPopup.SetActive(true);
        }
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

    public void OnMoleButtonClicked()
    {
        if (wak_GameManager.Instance != null)
        {
            wak_GameManager.Instance.ResetGameData();
        }

        Time.timeScale = 1;

        SceneManager.LoadScene("wak_PlayScene");
    }
}