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

public abstract class NewsPhaseInitializer : PhaseInitializer
{
    [SerializeField]
    GameObject newsObj = default;
    [SerializeField]
    Animator newsAnim = default;
    CancellationTokenSource _cts;

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
        if (targetphase == SetCurrnetPhase())
        {
            RoomNews(_cts.Token).Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    protected abstract GamePhase SetCurrnetPhase();
    protected abstract GamePhase SetNextPhase();

    private void OnDestroy()
    {
        _cts.Cancel();
    }

    async UniTask RoomNews(CancellationToken token)
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

        await Anim(token);

        FindObjectOfType<CommonManager>().LoadPhase(SetNextPhase());
        StopSound(token);
    }
    protected abstract void PlaySound();
    protected abstract UniTask Anim(CancellationToken token);
    protected abstract void StopSound(CancellationToken token);
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