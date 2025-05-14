using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseEyesPupilDataManager : MonoBehaviour
{
    public static BaseEyesPupilDataManager Instance { get; private set; }

    private List<EyePupilData> eyePupilDatas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        eyePupilDatas = new List<EyePupilData>();
    }

    public void SaveEyePupilData(List<EyePupilData> newData)
    {
        foreach (EyePupilData data in newData)
        {
            eyePupilDatas.Add(data);
        }
    }

    public void SavePlayerPrefs()
    {
        if (eyePupilDatas == null || eyePupilDatas.Count == 0)
        {
            return;
        }

        float leftAvg = eyePupilDatas.Average(data => data.leftPupilSize);
        float rightAvg = eyePupilDatas.Average(data => data.rightPupilSize);

        PlayerPrefs.SetFloat("left_avg", leftAvg);
        PlayerPrefs.SetFloat("right_avg", rightAvg);
        PlayerPrefs.Save();

        Debug.Log($"왼쪽 평균: {leftAvg}, 오른쪽 평균: {rightAvg} 저장 완료");
    }
}

public class EyePupilData
{
    public float leftPupilSize;
    public float rightPupilSize;

    public EyePupilData(float left, float right)
    {
        leftPupilSize = left;
        rightPupilSize = right;
    }
}