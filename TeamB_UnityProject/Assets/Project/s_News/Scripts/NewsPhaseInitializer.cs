﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public abstract class NewsPhaseInitializer : PhaseInitializer
{
    [SerializeField]
    GameObject newsObj = default;
    [SerializeField]
    Animator newsAnim = default;

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (targetphase == SetCurrnetPhase())
        {
            RoomNews().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    protected abstract GamePhase SetCurrnetPhase();
    protected abstract GamePhase SetNextPhase();

    async UniTask RoomNews()
    {
        // HandlePP(0.5f, 300f, 3f).Forget();

        if (newsObj == default)
        {
            Debug.LogAssertion("newsObjがnull");
        }
        else
        {
            newsObj.SetActive(false);//animリセット処理。
            newsObj.SetActive(true);
        }
        PlaySound();
        if (newsAnim == default)
        {
            Debug.LogAssertion("Animatorがnull");
        }
        else
        {
            newsAnim.SetTrigger("PlayNews");
        }
        Debug.Log("RoomNewsやってる");

        await Anim();

        FindObjectOfType<CommonManager>().LoadPhase(SetNextPhase());
        StopSound();
    }
    protected abstract void PlaySound();
    protected abstract UniTask Anim();
    protected abstract void StopSound();
    protected async UniTask FadeOutSound(AudioSource audio, float duration, CancellationToken token)
    {
        DOTween.To(() => audio.volume, (val) =>
        {
            audio.volume = val;
        }, 0, duration);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        audio.enabled = false;
    }

}