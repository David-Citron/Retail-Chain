using System;
using System.Collections;
using UnityEngine;

public class ActionTimer
{

    private Func<bool> predicate;

    private float totalTime;
    public float passedTime;

    private readonly float howOften;

    private readonly Action onUpdate;
    private readonly Action onFail;
    private readonly Action onComplete;
    private bool ended;

    public ActionTimer(Func<bool> predicate, Action onUpdate, Action onComplete, Action onFail, float totalTime, float howOften)
    {
        this.predicate = predicate;
        this.totalTime = totalTime;
        this.howOften = howOften;
        this.onUpdate = onUpdate;
        this.onComplete = onComplete;
        this.onFail = onFail;
    }
    public ActionTimer(Action onComplete, Action onFail, float totalTime, float howOften) : this(null, null, onComplete, onFail, totalTime, howOften) { }
    public ActionTimer(Action onComplete, float totalTime, float howOften) : this(null, onComplete, totalTime, howOften) { }
    public ActionTimer(Func<bool> predicate, Action onComplete, float totalTime, float howOften) : this(predicate, null, onComplete, null, totalTime, howOften) { }
    public ActionTimer(Func<bool> predicate, Action onComplete, Action onFail, float totalTime, float howOften) : this(predicate, null, onComplete, onFail, totalTime, howOften) { }

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
            onUpdate?.Invoke();
        }

        if (ended) yield break;
        ended = true;
        onComplete?.Invoke();
    }
}