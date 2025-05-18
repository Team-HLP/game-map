using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialManager2 : MonoBehaviour
{
    public GameObject TutorialSkipManager;

    void Start()
    {
        if (TutorialSkipManager != null)
        {
            TutorialSkipManager.SetActive(false);
        }
        string gameScene = PlayerPrefs.GetString("gameScene", "");
        StartCoroutine(ExistGamePlayHistory(gameScene));
    }

    IEnumerator ExistGamePlayHistory(string gameScene)
    {
        string param = (gameScene == "MeteoriteScene") ? "METEORITE_DESTRUCTION" : "CATCH_MOLE";
        string url = Apiconfig.url + "/games/exist?gameCategory=" + param;

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token", ""));
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ExistGameResponse res = JsonUtility.FromJson<ExistGameResponse>(request.downloadHandler.text);
            if (!res.exists)
            {
                TutorialSkipManager.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                TutorialSkipManager.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    [System.Serializable]
    public class ExistGameResponse
    {
        public bool exists;
    }
}
