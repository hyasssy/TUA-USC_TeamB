using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

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
        if(targetphase == GamePhase.Room1){
            RoomMain();
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }

    private void OnDestroy() {
        _cts.Cancel();
    }

    async UniTask RoomNews(CancellationToken token){
        if(newsObj == default){
            Debug.LogAssertion("newsObjがnull");
        }else{
            newsObj.SetActive(false);//animリセット処理。
            newsObj.SetActive(true);
        }
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
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Room1);
    }
    void RoomMain(){
        //選択可能をリセットする




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
