using System;
using System.Collections;
using UnityEngine;

public class ActionTimer
{

    private Func<bool> predicate;

    private float totalTime;
    private float passedTime;

    private readonly float howOften;

    private readonly Action whenDone;
    private readonly Action onFail;
    private bool ended;


    public ActionTimer(Action whenDone, float totalTime, float howOften) : this(null, whenDone, totalTime, howOften)
    {}

    public ActionTimer(Func<bool> predicate, Action whenDone, float totalTime, float howOften) :
        this(predicate, whenDone, null, totalTime, howOften)
    {}

    public ActionTimer(Func<bool> predicate, Action whenDone, Action onFail, float totalTime, float howOften)
    {
        this.predicate = predicate;
        this.totalTime = totalTime;
        this.howOften = howOften;
        this.whenDone = whenDone;
        this.onFail = onFail;
    }

    public ActionTimer Run()
    {
        if (predicate != null) PlayerManager.instance.StartCoroutine(CheckFailPrediction());
        PlayerManager.instance.StartCoroutine(RunAction());
        return this;
    }

    private IEnumerator CheckFailPrediction()
    {
        while (!ended)
        {
            yield return new WaitForEndOfFrame();
            if (predicate.Invoke()) continue;

            onFail?.Invoke();
            ended = true;
        }
    }

    private IEnumerator RunAction()
    {
        while (passedTime < totalTime)
        {
            if (ended) yield break;

            yield return new WaitForSecondsRealtime(howOften);
            passedTime += howOften;
        }

        ended = true;
        whenDone?.Invoke();
    }
}