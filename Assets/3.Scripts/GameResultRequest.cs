[System.Serializable]
public class GameResultRequest
{
    public int meteorite_prefab_count;
    public int fuel_prefab_count;
    public string result;
    public int score;
    public int hp;
    public int meteorite_broken_count;

    public GameResultRequest(int meteorite_prefab_count, int fuel_prefab_count, string result, int score, int hp, int meteorite_broken_count)
    {
        this.meteorite_prefab_count = meteorite_prefab_count;
        this.fuel_prefab_count = fuel_prefab_count;
        this.result = result;
        this.score = score;
        this.hp = hp;
        this.meteorite_broken_count = meteorite_broken_count;
    }
}