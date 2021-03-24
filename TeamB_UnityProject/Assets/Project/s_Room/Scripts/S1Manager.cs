using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class S1Manager : RoomPhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    [SerializeField]
    AudioSource audioSource = default, newsBGM = default, mainBGM = default, ambient = default;
    [SerializeField]
    AudioClip newsSound = default, mainSound = default;
    int flagAmount;
    public override void InitializePhase(GamePhase targetphase){
        Debug.Log("InitializePhase");
        if(!ambient.isPlaying){
            ambient.Play();
        }
        if(targetphase == GamePhase.s1_Room_News){
            RoomNews(this.GetCancellationTokenOnDestroy()).Forget();
        }else if(targetphase == GamePhase.s1_Room_Main){
            RoomMain();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    public override void CheckFlag(){
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if(flag >= flagAmount){
            FadeOutAudio(mainBGM, 1f).Forget();
            FindObjectOfType<CommonManager>().LoadPhase(GamePhase.s2_Dog_1);
        }
    }

    async UniTask RoomNews(CancellationToken token){
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        //音鳴らす
        newsBGM.Play();
        //アニメーションでもいいかも。timeline
        //カメラ移動

        //ズームアウト

        //カメラ移動

        //音消す
        newsBGM.Stop();

        //最後にMainに進む
        await UniTask.Delay(5000, false, PlayerLoopTiming.Update, token);
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.s1_Room_Main);
    }
    void RoomMain(){
        flag = 0;
        flagAmount = FindObjectsOfType<RoomObj>().Length;
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
