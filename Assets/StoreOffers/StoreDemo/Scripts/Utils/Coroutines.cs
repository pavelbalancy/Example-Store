using System;
using System.Collections;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    private static Coroutines _script;
    private static readonly WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate();

    private static Coroutines Script
    {
        get
        {
            if (_script == null)
            {
                var go = new GameObject("Coroutines");
                DontDestroyOnLoad(go);
                _script = go.AddComponent<Coroutines>();
            }

            return _script;
        }
    }

    public static Coroutines Instance => Script;

    public static Coroutine Wait(float delay, Action callback)
    {
        if (delay <= 0.00001f)
        {
            callback?.Invoke();
            return null;
        }

        return Script.StartCoroutine(WaitForTime(delay, callback));
    }

    public static Coroutine WaitUntil(Func<bool> condition, Action callback)
    {
        return Script.StartCoroutine(WaitUntilInternal(condition, callback));
    }

    private static IEnumerator WaitUntilInternal(Func<bool> condition, Action callback)
    {
        while (!condition())
        {
            yield return FixedUpdate;
        }

        callback?.Invoke();
    }

    public static Coroutine WaitOneFrame(Action callback)
    {
        return Script.StartCoroutine(WaitOneFramePrivate(callback));
    }

    public static Coroutine WaitTwoFrames(Action callback)
    {
        return Script.StartCoroutine(WaitTwoFramesPrivate(callback));
    }

    public static Coroutine Periodic(float period, float duration, Action callback)
    {
        if (duration < period / 2)
            return null;

        return Script.StartCoroutine(WaitForPeriod(period, Mathf.RoundToInt(duration / period), callback));
    }

    public static void StopCoroutineRemotely(Coroutine coroutine)
    {
        if (coroutine != null)
            Script.StopCoroutine(coroutine);
    }

    public static Coroutine EveryFrame(float duration, Action<float> callback, Action doneCallback)
    {
        return Script.StartCoroutine(StartFrames(duration, callback, doneCallback));
    }

    private static IEnumerator WaitForTime(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    private static IEnumerator WaitOneFramePrivate(Action callback)
    {
        yield return null;
        callback?.Invoke();
    }

    private static IEnumerator WaitTwoFramesPrivate(Action callback)
    {
        yield return null;
        yield return null;
        callback?.Invoke();
    }

    private static IEnumerator WaitForPeriod(float period, int count, Action callback)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(period);
            callback?.Invoke();
        }
    }

    private static IEnumerator StartFrames(float duration, Action<float> callback, Action doneCallback)
    {
        float t = 0;
        callback?.Invoke(0);
        while (t < duration)
        {
            yield return null;
            t += Time.deltaTime;
            callback?.Invoke(t / duration);
        }

        doneCallback?.Invoke();
    }
}