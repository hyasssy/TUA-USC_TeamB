using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class News0Manager : NewsPhaseInitializer
{
    [SerializeField]
    float newsDuration = 15f;
    [SerializeField]
    AudioSource audioSource = default, newsBGM = default, ambient = default;
    Volume _volume;
    Bloom _bloom;

    override protected void PlaySound(){
        //音鳴らす
        newsBGM.enabled = true;
        newsBGM.Play();
        Debug.Log("Play");
        // ambientは通しで流そうかみたいな考えがあった名残。
        // if(!ambient.isPlaying){
        //     ambient.Play();
        // }
    }
    protected override GamePhase SetPhase(){
        return GamePhase.News0;
    }

    override protected async UniTask Anim(CancellationToken token){
        //アニメーション制御の要素：newsのscale、bloom、AudioVolume、プチって切り替わって顔が映るトリガー
        var _nap = FindObjectOfType<NewsAnimParam>();
        if(_nap == null){
            Debug.LogAssertion("NewsAnimParamが見つかりません。");
        }
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
    }
    override protected void StopSound(CancellationToken token){
        float duration = 1.5f;
        FadeOutSound(newsBGM, duration, token).Forget();
    }
}