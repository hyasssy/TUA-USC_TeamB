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


public class OpeningManager : PhaseInitializer
{
    //言語選択
    [SerializeField]
    GameObject newsObj = default;
    [SerializeField]
    Animator newsAnim = default;

    protected override void InitializePhase(GamePhase targetphase)
    {
        Debug.Log("InitializePhase");
        if (targetphase == GamePhase.Opening)
        {
            RoomNews().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }


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

        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.News0);
        StopSound();
    }
    void PlaySound()
    {

    }
    async UniTask Anim()
    {
        await UniTask.Yield();
    }
    void StopSound()
    {

    }
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
