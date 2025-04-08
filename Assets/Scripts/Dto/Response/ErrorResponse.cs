using UnityEngine;

[System.Serializable]
public class ErrorResponse
{
    public string detail;

    public static string ParseErrorMessage(string json)
    {
        Debug.Log($"서버 응답 원문 (에러): {json}");

        try
        {
            return JsonUtility.FromJson<ErrorResponse>(json).detail;
        }
        catch
        {
            return "서버에서 오류가 발생했습니다.";
        }
    }
}
