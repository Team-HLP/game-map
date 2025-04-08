using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField inputId;
    public TMP_InputField inputPassword;
    public TextMeshProUGUI resultText;
    public string loginUrl = "http://localhost:8000/user/login";

    public void OnLoginButtonClicked()
    {
        string email = inputId.text;
        string password = inputPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            resultText.text = "아이디와 비밀번호를 입력해주세요.";
            return;
        }

        string hashedPassword = HashUtil.ComputeSHA256(password);

        StartCoroutine(LoginCoroutine(email, hashedPassword));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        string jsonBody = JsonUtility.ToJson(new LoginRequest(email, password));

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
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

            resultText.text = "로그인 성공!";
        }
        else
        {
            string errorMsg = ErrorResponse.ParseErrorMessage(request.downloadHandler.text);
            resultText.text = $"{errorMsg}";
        }
    }
}
