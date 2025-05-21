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

        var leftSorted = eyePupilDatas.Select(data => data.leftPupilSize).OrderBy(x => x).ToList();
        var rightSorted = eyePupilDatas.Select(data => data.rightPupilSize).OrderBy(x => x).ToList();

        float leftMedian;
        float rightMedian;
        int count = leftSorted.Count;

        if (count % 2 == 1)
        {
            leftMedian = leftSorted[count / 2];
            rightMedian = rightSorted[count / 2];
        }
        else
        {
            leftMedian = (leftSorted[(count / 2) - 1] + leftSorted[count / 2]) / 2f;
            rightMedian = (rightSorted[(count / 2) - 1] + rightSorted[count / 2]) / 2f;
        }

        PlayerPrefs.SetFloat("left_avg", leftMedian);
        PlayerPrefs.SetFloat("right_avg", rightMedian);
        PlayerPrefs.Save();
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