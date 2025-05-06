using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DotSpawner : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject dotPrefab;
    public GameObject retryFocusNoticeText;
    public GameObject complimentPhrases;

    private const int totalSessions = 3;
    private int currentSession = 0;
    private bool isFisrtDisPlayRetryFocusNoticeText = false;

    void Start()
    {
        SpawnDot();
    }

    public void SpawnDot()
    {
        if (currentSession >= totalSessions) return;

        Vector3 spawnPosition = GetRandomWorldPoint();
        GameObject dot = Instantiate(dotPrefab, spawnPosition, Quaternion.identity, canvasRect);

        DotVisual dotScript = dot.GetComponent<DotVisual>();
        dotScript.Init(this);
        PositionRetryFocusAbove(dot.transform);
    }

    public void ActiveRetryFocusNoticeText()
    {
        if (!isFisrtDisPlayRetryFocusNoticeText)
        {
            return;
        }
        if (!retryFocusNoticeText.activeSelf)
        {
            retryFocusNoticeText.SetActive(true);
        }
    }

    public void DisActiveRetryFocusNoticeText()
    {
        if (!isFisrtDisPlayRetryFocusNoticeText)
        {
            retryFocusNoticeText.SetActive(false);
            isFisrtDisPlayRetryFocusNoticeText = true;
        }
        else if (retryFocusNoticeText.activeSelf)
        {
            retryFocusNoticeText.SetActive(false);
        }
    }

    public void OnDotCompleted()
    {
        if (++currentSession < totalSessions)
        {
            SpawnDot();
        }
        else
        {
            IntroConfirmHandler.Instance.rollBack();
            StartCoroutine(ShowComplimentAndLoadNextScene());
        }
    }

    private IEnumerator ShowComplimentAndLoadNextScene()
    {
        if (complimentPhrases != null)
        {
            complimentPhrases.SetActive(true);
        }

        yield return new WaitForSeconds(5f);

        BaseEyesPupilDataManager.Instance.SavePlayerPrefs();
        SceneManager.LoadScene(PlayerPrefs.GetString("gameScene", "MENU"));
    }

    Vector3 GetRandomWorldPoint()
    {
        Vector2 localPos = new Vector2(
            Random.Range(-canvasRect.rect.width / 2, canvasRect.rect.width / 2),
            Random.Range(-canvasRect.rect.height / 2, canvasRect.rect.height / 2)
        );

        return canvasRect.TransformPoint(localPos);
    }

    private void PositionRetryFocusAbove(Transform dotTransform)
    {
        if (retryFocusNoticeText != null)
        {
            Transform noticeTransform = retryFocusNoticeText.transform;
            Vector3 aboveDotPos = dotTransform.position + new Vector3(0, 60f, 0);

            noticeTransform.position = aboveDotPos;
            retryFocusNoticeText.SetActive(false);
        }
    }
}