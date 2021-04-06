using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class RoomPhaseInitializer : PhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    int flagAmount;
    CancellationTokenSource _cts;

    public override void InitializePhase(GamePhase targetphase){
        if(_cts == null){
            _cts = new CancellationTokenSource();
        }else{
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }

        Debug.Log("InitializePhase");
        if(targetphase == SetPhase()){
            RoomMain();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    abstract protected GamePhase SetPhase();
    void RoomMain(){
        FadeManager.FadeIn();
        //選択可能をリセットする
        flag = 0;
        var roomObjs = FindObjectsOfType<RoomObj>();

        roomObjs.ToList().ForEach(r => {
            //重要なものだけカウントする。
            if(r.IsImportant) flagAmount++;
            r.SetUpRoomItem();
        });
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        PlaySound();
    }
    private void OnDisable() {
        if(_cts != null){
            _cts.Cancel();
        }
    }
    public void CheckFlag(){
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if(flag >= flagAmount){
            LoadNextScene();
        }
    }
    abstract protected void PlaySound();
    abstract protected void LoadNextScene();
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
