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
    GameObject newsObj = default;
    [SerializeField]
    AudioClip newsSound = default, mainSound = default;
    [SerializeField]
    Animator newsAnim = default;
    [SerializeField]
    float newsDuration = 15f;
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
        if(!ambient.isPlaying){
            ambient.Play();
        }
        if(targetphase == GamePhase.s1_Room_News){
            RoomNews(_cts.Token).Forget();
        }else if(targetphase == GamePhase.s1_Room_Main){
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
            FindObjectOfType<CommonManager>().LoadPhase(GamePhase.s2_Dog_1);
        }
    }

    async UniTask RoomNews(CancellationToken token){
        if(newsObj == default){
            Debug.LogAssertion("newsObjがnull");
        }else{
            newsObj.SetActive(false);//animリセット処理。
            newsObj.SetActive(true);
        }
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        //音鳴らす
        newsBGM.Play();
        if(newsAnim == default){
            Debug.LogAssertion("Animatorがnull");
        }else{
            newsAnim.SetTrigger("PlayNews");
        }
        Debug.Log("RoomNewsやってる");

        //音消す
        newsBGM.Stop();

        //最後にMainに進む
        await UniTask.Delay((int)(newsDuration * 1000), false, PlayerLoopTiming.Update, token);
        FadeManager.FadeOut();
        await UniTask.Delay(1500, false, PlayerLoopTiming.Update, token);
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.s1_Room_Main);
    }
    void RoomMain(){
        if(newsObj == default){
            Debug.LogAssertion("newsObjがnull");
        }else{
            newsObj.SetActive(false);
        }
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
