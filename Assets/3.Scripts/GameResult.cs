[System.Serializable]
public class GameResult
{
    public int hp;
    public bool success;
    public int score;
    public int destroyedMeteo;
    public float gameTime;
    public int index;

    public GameResult(int index, int hp, bool success, int score, int destroyedMeteo, float gameTime)
    {
        this.index = index;
        this.hp = hp;
        this.success = success;
        this.score = score;
        this.destroyedMeteo = destroyedMeteo;
        this.gameTime = gameTime;
    }
}