using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager2 : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("wak_PlayScene");
    }
    public void OnQuitButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameData();
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("MENU");
    }
    public void OnResultButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("ResultScene");
    }
}