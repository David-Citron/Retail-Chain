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

    public int duration;
    public int remainingDuration;
    public bool pause = false;

    private void Start()
    {
        instance = this;

        circleTimer.SetActive(false);
        doneMarker.SetActive(false);
    }

    public static void Start(int seconds)
    {
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

            // Smoothly update the fill amount over 1 second
            while (elapsed < 1f)
            {
                if (pause) yield break;

                elapsed += Time.unscaledDeltaTime;

                // Calculate fill based on total elapsed time relative to total duration
                float totalElapsedTime = totalDuration - remainingDuration + elapsed;
                fill.fillAmount = Mathf.InverseLerp(0, totalDuration, totalElapsedTime);

                // Use the current remaining duration for the text
                text.text = $"{(remainingDuration / 60):00}:{(remainingDuration % 60):00}";

                yield return null; // Wait for the next frame
            }

            // Decrement the remaining duration after the smooth transition
            remainingDuration--;
        }

        doneMarker.SetActive(true);
        text.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        circleTimer.SetActive(false);
    }

    public static void Stop()
    {
        instance.pause = true;
        instance.circleTimer.SetActive(false);
    }
}
