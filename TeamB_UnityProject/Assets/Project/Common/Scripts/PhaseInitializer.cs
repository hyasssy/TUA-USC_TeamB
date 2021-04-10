using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

//抽象クラスでInitializePhaseを実装。
public abstract class PhaseInitializer : MonoBehaviour
{
    protected CancellationTokenSource cts;
    private void Start()
    {
        var list = FindObjectsOfType<PhaseInitializer>().ToList();
        //たくさんあるとまずい。シーン上に一つの想定
        list.ForEach(obj => Debug.Log("SceneInitializeを持ったオブジェクト:" + obj.gameObject));
    }
    public void InitPhase(GamePhase currentphase)
    {
        if (cts == null)
        {
            cts = new CancellationTokenSource();
        }
        else
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
        }
        Debug.Log("InitializePhase");
        InitializePhase(currentphase);
    }
    abstract protected void InitializePhase(GamePhase currentphase);
    private void OnDestroy()
    {
        cts.Cancel();
    }
    protected async UniTask FadeOutSound(AudioSource audio, float duration)
    {
        var t = 0f;
        var primaryVolume = audio.volume;
        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = primaryVolume * (1 - (t / duration));
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        }
        audio.enabled = false;
    }
    protected async UniTask FadeInSound(AudioSource audioSource, float delay, float duration)
    {
        await UniTask.Delay((int)(delay * 1000), cancellationToken: cts.Token);
        var originalVolume = audioSource.volume;
        audioSource.volume = 0f;
        audioSource.Play();
        var t = 0f;
        while (t < duration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
            t += Time.deltaTime;
            var p = Easing.QuadInOut(t, duration, 0, 1);
            audioSource.volume = p * originalVolume;
        }
    }
}