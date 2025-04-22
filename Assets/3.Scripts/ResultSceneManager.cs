using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        GameManager.Instance.ResetGameData();
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");
    }
    public void OnQuitButtonClicked()
    {
        GameManager.Instance.ResetGameData();
        SceneManager.LoadScene("MENU");
    }
    public void OnResultButtonClicked()
    {
        SceneManager.LoadScene("ResultScene");
    }
}