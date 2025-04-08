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
