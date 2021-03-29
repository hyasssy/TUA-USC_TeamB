using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Room1Manager : RoomPhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    [SerializeField]
    AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    // [SerializeField]
    // AudioClip mainSound = default;
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
        if(targetphase == GamePhase.Room1){
            RoomMain();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    private void OnDisable() {
        _cts.Cancel();
    }
    public override void CheckFlag(){
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if(flag >= flagAmount){
            FadeOutAudio(radio, 1f).Forget();
            FadeOutAudio(radioNoise, 1f).Forget();
            FadeOutAudio(ambient, 1f).Forget();
            FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Dog1);
        }
    }
    void RoomMain(){
        FadeManager.FadeIn();
        //選択可能をリセットする
        flag = 0;
        var roomObjs = FindObjectsOfType<RoomObj>();
        flagAmount = roomObjs.Length;
        roomObjs.ToList().ForEach(r => r.SetUpRoomItem());
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        radio.Play();
        radioNoise.Play();
        ambient.Play();
    }
    async UniTask FadeOutAudio(AudioSource audio, float duration){
        var t = 0f;
        var primaryVolume = audio.volume;
        while(t < duration){
            t += Time.deltaTime;
            audio.volume = primaryVolume * (1 - (t / duration));
            await UniTask.Yield();
        }
        audio.enabled = false;
    }
}