﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class DogPhaseInitializer : PhaseInitializer
{
    CancellationTokenSource _cts;
    protected float strokeSum = 0;
    HandController handController;
    //これはHandControllerのIsClickableとやや役割が被っている。
    protected bool stopStroke = false;
    public void StrokingDog()
    {
        if (stopStroke) return;
        var value = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
        if (value != 0)
        {
            strokeSum += value;
            Debug.Log("Stroke dog " + value + ", amount = " + strokeSum);
            StrokeEvent(_cts.Token);
        }
    }
    protected abstract void StrokeEvent(CancellationToken token);

    public override void InitializePhase(GamePhase targetphase)
    {
        if (_cts == null)
        {
            _cts = new CancellationTokenSource();
        }
        else
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        Debug.Log("InitializePhase");
        if (targetphase == GamePhase.Dog1)
        {
            DogMain();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    void DogMain()
    {
        FadeManager.FadeIn();
        //クリックできるかを初期化する。
        handController = FindObjectOfType<HandController>();
        handController.SwitchClickable(true);
        stopStroke = false;
        //各シーンごとの初期化
        DogInit();
    }
    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }
    }
    abstract protected void DogInit();
    protected async UniTask FadeOutSound(AudioSource audio, float duration)
    {
        var t = 0f;
        var primaryVolume = audio.volume;
        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = primaryVolume * (1 - (t / duration));
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
        }
        audio.enabled = false;
    }
}
