using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircleTimer : MonoBehaviour
{
    private static CircleTimer instance;

    [SerializeField] private GameObject circleTimer;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject doneMarker;

    public float duration;
    public float remainingDuration;
    public bool pause = false;

    private void Start()
    {
        instance = this;

        circleTimer.SetActive(false);
        doneMarker.SetActive(false);
    }

    public static void Start(float seconds)
    {
        Stop();
        instance.doneMarker.SetActive(false);
        instance.circleTimer.SetActive(true);
        instance.text.gameObject.SetActive(true);

        instance.duration = seconds;
        instance.remainingDuration = seconds;
        instance.pause = false;
        instance.StartCoroutine(instance.UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        float totalDuration = duration; // Store the initial duration for reference

        while (remainingDuration > 0)
        {
            if (pause) yield break;

            float elapsed = 0f;

            while (elapsed < 1f)
            {
                if (pause) yield break;

                elapsed += Time.unscaledDeltaTime;

                float totalElapsedTime = totalDuration - remainingDuration + elapsed;
                fill.fillAmount = Mathf.InverseLerp(0, totalDuration, totalElapsedTime);

                text.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";

                yield return null;
            }

            remainingDuration--;
        }

        fill.fillAmount = 0;
        doneMarker.SetActive(true);
        text.gameObject.SetActive(false);
        yield return new WaitForSeconds(.3f);
        if(remainingDuration == 0) circleTimer.SetActive(false);
    }

    public static void Stop()
    {
        instance.pause = true;
        instance.circleTimer.SetActive(false);
    }
}
