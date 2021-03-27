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

public class News1Manager : PhaseInitializer
{
    [SerializeField]
    AudioSource audioSource = default, newsBGM = default, ambient = default;
    [SerializeField]
    GameObject newsObj = default;
    [SerializeField]
    Animator newsAnim = default;
    [SerializeField]
    float newsDuration = 15f;
    [SerializeField]
    float startAudioVolume = 1f;
    CancellationTokenSource _cts;
    Volume _volume;
    Bloom _bloom;
    public override void InitializePhase(GamePhase targetphase){
        if(_cts == null){
            _cts = new CancellationTokenSource();
        }else{
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
        Debug.Log("InitializePhase");
        if(targetphase == GamePhase.News1){
            RoomNews(_cts.Token).Forget();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }

    private void OnDestroy() {
        _cts.Cancel();
    }

    async UniTask RoomNews(CancellationToken token){
        HandlePP(0.5f, 100f, 6f).Forget();

        if(!ambient.isPlaying){
            ambient.Play();
        }
        if(newsObj == default){
            Debug.LogAssertion("newsObjがnull");
        }else{
            newsObj.SetActive(false);//animリセット処理。
            newsObj.SetActive(true);
        }
        //音鳴らす
        newsBGM.enabled = true;
        newsBGM.volume = startAudioVolume;
        newsBGM.Play();
        Debug.Log("Play");
        if(newsAnim == default){
            Debug.LogAssertion("Animatorがnull");
        }else{
            newsAnim.SetTrigger("PlayNews");
        }
        Debug.Log("RoomNewsやってる");

        //音消す
        // newsBGM.Stop();

        //最後にMainに進む
        await UniTask.Delay((int)(newsDuration * 1000), false, PlayerLoopTiming.Update, token);
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Room1);
        FadeOutAudio(newsBGM, 1.5f, token).Forget();
    }
    async UniTask HandlePP(float delay, float startBloomIntensity, float duration){
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet<Bloom>(out _bloom);
        var p = _bloom.intensity.value;
        _bloom.intensity.value = startBloomIntensity;
        await UniTask.Delay((int)(delay * 1000), cancellationToken: _cts.Token);
        DOTween.To(() => startBloomIntensity, (val) =>
        {
            _bloom.intensity.value = val;
        }, p, duration).SetEase(Ease.InOutQuad);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: _cts.Token);
        DOTween.To(() => p, (val) =>
        {
            _bloom.intensity.value = val;
        }, 2.5f, 7f).SetEase(Ease.InOutSine);
    }
    async UniTask FadeOutAudio(AudioSource audio, float duration, CancellationToken token){
        DOTween.To(() => audio.volume, (val) =>
        {
            audio.volume = val;
        }, 0, duration);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        audio.enabled = false;
    }
}