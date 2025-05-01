[System.Serializable]
public class GameResultRequest
{
    public string result;
    public int score;
    public int hp;
    public int meteorite_broken_count;

    public GameResultRequest(string result, int score, int hp, int meteorite_broken_count)
    {
        this.result = result;
        this.score = score;
        this.hp = hp;
        this.meteorite_broken_count = meteorite_broken_count;
    }
}