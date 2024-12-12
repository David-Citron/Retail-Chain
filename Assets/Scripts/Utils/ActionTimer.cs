using System;
using System.Collections;
using UnityEngine;

public class ActionTimer : MonoBehaviour
{

    private Func<bool> predicate;

    private int totalTime;
    private int passedTime;

    private readonly int howOften;

    private readonly Action whenDone;
    private readonly Action onFail;

    public ActionTimer(Action whenDone, int totalTime, int howOften) : this(null, whenDone, totalTime, howOften)
    {}

    public ActionTimer(Func<bool> predicate, Action whenDone, int totalTime, int howOften) :
        this(predicate, whenDone, null, totalTime, howOften)
    {}

    public ActionTimer(Func<bool> predicate, Action whenDone, Action onFail, int totalTime, int howOften)
    {
        this.predicate = predicate;
        this.totalTime = totalTime;
        this.howOften = howOften;
        this.whenDone = whenDone;
        this.onFail = onFail;
    }


    public IEnumerator Run()
    {
        while (passedTime < totalTime)
        {
            if (predicate != null && !predicate.Invoke())
            {
                onFail?.Invoke();
                yield break;
            }

            yield return new WaitForSecondsRealtime(howOften);
            passedTime += howOften;
        }

        whenDone?.Invoke();
    }
}