using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class DogPhaseInitializer : PhaseInitializer
{
    CancellationTokenSource _cts;

    public override void InitializePhase(GamePhase targetphase){
        if(_cts == null){
            _cts = new CancellationTokenSource();
        }else{
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        Debug.Log("InitializePhase");
        if(targetphase == GamePhase.Dog1){
            DogMain();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    void DogMain(){
        FadeManager.FadeIn();
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        // PlaySound();
    }
    private void OnDisable() {
        if(_cts != null){
            _cts.Cancel();
        }
    }
    public void CheckFlag(){
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        LoadNextScene();
    }
    // abstract protected void PlaySound();
    // abstract protected void LoadNextScene();
    void LoadNextScene(){

    }
    protected async UniTask FadeOutSound(AudioSource audio, float duration){
        var t = 0f;
        var primaryVolume = audio.volume;
        while(t < duration){
            t += Time.deltaTime;
            audio.volume = primaryVolume * (1 - (t / duration));
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken:_cts.Token);
        }
        audio.enabled = false;
    }
}
