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
    CancellationTokenSource _cts;
    //アニメーション制御の要素：newsのscale、bloom、AudioVolume、プチって切り替わって顔が映るトリガー
    NewsAnimParam _nap;



    [SerializeField]
    float newsDuration = 15f;
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
        _nap = FindObjectOfType<NewsAnimParam>();
        // HandlePP(0.5f, 300f, 3f).Forget();

        // if(!ambient.isPlaying){
        //     ambient.Play();
        // }
        if(newsObj == default){
            Debug.LogAssertion("newsObjがnull");
        }else{
            newsObj.SetActive(false);//animリセット処理。
            newsObj.SetActive(true);
        }
        //音鳴らす
        newsBGM.enabled = true;
        newsBGM.Play();
        Debug.Log("Play");
        if(newsAnim == default){
            Debug.LogAssertion("Animatorがnull");
        }else{
            newsAnim.SetTrigger("PlayNews");
        }
        Debug.Log("RoomNewsやってる");

        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet<Bloom>(out _bloom);
        var p = _bloom.intensity.value;
        float t = 0f;
        while(t<newsDuration){
            //Set Animating Parameter
            newsBGM.volume = _nap.currentAudioVolume;
            _bloom.intensity.value = _nap.currentBloomIntensity;
            _nap.transform.localScale = new Vector3(_nap.currentScale, _nap.currentScale, 1f);
            t += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }


        //最後にMainに進む
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Room1);
        FadeOutAudio(newsBGM, 1.5f, token).Forget();
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