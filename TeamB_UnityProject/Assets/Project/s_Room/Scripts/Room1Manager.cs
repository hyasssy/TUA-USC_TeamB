﻿using System.Collections;
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
    AudioSource audioSource = default, mainBGM = default, ambient = default;
    [SerializeField]
    AudioClip mainSound = default;
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
    private void OnDestroy() {
        _cts.Cancel();
    }
    public override void CheckFlag(){
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if(flag >= flagAmount){
            FadeOutAudio(mainBGM, 1f).Forget();
            FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Dog1);
        }
    }

    // async UniTask RoomNews(CancellationToken token){
    //     if(newsObj == default){
    //         Debug.LogAssertion("newsObjがnull");
    //     }else{
    //         newsObj.SetActive(false);//animリセット処理。
    //         newsObj.SetActive(true);
    //     }
    //     FindObjectOfType<RoomHandController>().SwitchClickable(false);
    //     //音鳴らす
    //     newsBGM.Play();
    //     if(newsAnim == default){
    //         Debug.LogAssertion("Animatorがnull");
    //     }else{
    //         newsAnim.SetTrigger("PlayNews");
    //     }
    //     Debug.Log("RoomNewsやってる");

    //     //音消す
    //     newsBGM.Stop();

    //     //最後にMainに進む
    //     await UniTask.Delay((int)(newsDuration * 1000), false, PlayerLoopTiming.Update, token);
    //     FadeManager.FadeOut();
    //     await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);
    //     FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Room1);
    // }
    void RoomMain(){
        FadeManager.FadeIn();
        if(!ambient.isPlaying){
            ambient.Play();
        }
        //選択可能をリセットする

        // if(newsObj == default){
        //     Debug.LogAssertion("newsObjがnull");
        // }else{
        //     newsObj.SetActive(false);
        // }
        flag = 0;
        var roomObjs = FindObjectsOfType<RoomObj>();
        flagAmount = roomObjs.Length;
        //選択判定をリセットする。
        roomObjs.ToList().ForEach(r => r.SetUpRoomItem());
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        mainBGM.Play();
    }
    async UniTask FadeOutAudio(AudioSource audio, float duration){
        float t = 0;
        while(t < duration){
            t += Time.deltaTime;
            audio.volume = 1 - (t / duration);
            await UniTask.Yield();
        }
        audio.enabled = false;
    }
}
