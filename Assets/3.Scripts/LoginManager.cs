using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField idInput;    // ID 입력창으로 변경
    public TMP_InputField passwordInput; // TMP_InputField로 변경
    private string nextSceneName = "SelectGameScene"; // 로그인 성공 후 전환될 씬 이름

    public void OnLoginButtonClick()
    {
        string id = "user" + idInput.text;
        string password = passwordInput.text;

        StartCoroutine(LoginCoroutine(id, password));
    }

    IEnumerator LoginCoroutine(string id, string password)
    {
        string jsonBody = JsonUtility.ToJson(new LoginRequest(id, password));

        UnityWebRequest request = new UnityWebRequest(Apiconfig.url + "/user/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            PlayerPrefs.SetString("access_token", response.access_token);
            PlayerPrefs.Save();
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            string errorMsg = ParseErrorMessage(request.downloadHandler.text);
            Debug.Log(errorMsg);
        }
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string login_id;
        public string password;

        public LoginRequest(string loginId, string password)
        {
            this.login_id = loginId;
            this.password = password;
        }
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string access_token;
    }

    string ParseErrorMessage(string json)
    {
        Debug.Log($"서버 응답 원문 (에러): {json}");

        try
        {
            return JsonUtility.FromJson<ErrorResponse>(json).detail;
        }
        catch
        {
            return "알 수 없는 오류가 발생했습니다.";
        }
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public string detail;
    }
}
