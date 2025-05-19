using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialManager : MonoBehaviour
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
            if (res.GetExists())
            {
                TutorialSkipManager.SetActive(false);
            }
            else
            {
                TutorialSkipManager.SetActive(true);
            }
        }
    }

    [System.Serializable]
    public class ExistGameResponse
    {
        public bool exist;

        public bool GetExists()
        {
            return exist;
        }
    }
}
