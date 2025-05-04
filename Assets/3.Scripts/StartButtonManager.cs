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
        PlayerPrefs.SetString("gameScene", "MeteoriteScene");
        PlayerPrefs.Save();
        SceneManager.LoadScene("EyeDataMeausreScene");
    }
}