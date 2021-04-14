using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public abstract class RoomPhaseInitializer : PhaseInitializer
{
    [Serializable]
    protected class EventParamSets
    {
        public string objName = default;
        public List<EventParamSet> eventParams = default;
        public List<EventParamSet> eventParams_latter = default;
    }
    [SerializeField]
    protected List<EventParamSets> objParams = default;

    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    int flagAmount;
    [Serializable]
    protected class DialogueParams
    {
        public float[] timeCount = default;
        [TextArea(1, 4)]
        public string[] dialogues = default;
    }
    [SerializeField]
    protected DialogueParams firstTexts_ja = default, firstTexts_en = default;
    protected DialogueParams firstTexts;
    [SerializeField]
    protected DialogueParams endTexts_ja = default, endTexts_en = default;
    protected DialogueParams endTexts;
    [SerializeField]
    float textAppearDuration = 0.1f;


    protected override void InitializePhase(GamePhase targetphase)
    {
        if (targetphase == SetPhase())
        {
            RoomMain();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    abstract protected GamePhase SetPhase();
    void RoomMain()
    {
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        firstTexts = lang == Lang.ja ? firstTexts_ja : firstTexts_en;
        endTexts = lang == Lang.ja ? endTexts_ja : endTexts_en;
        SetObjParams();

        FadeManager.FadeIn();
        FindObjectOfType<PlayerCamController>().SetDefaultCamera();
        FindObjectOfType<SubtitleCanvas>().SetUpTexts();
        //選択可能をリセットする
        flag = 0;
        var roomObjs = FindObjectsOfType<RoomObj>();

        roomObjs.ToList().ForEach(r =>
        {
            //重要なものだけカウントする。
            if (r.IsImportant) flagAmount++;
            r.SetUpRoomItem();
        });
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        // PlaySound();
        //テキスト出すなど、シーン最初のイベントを設定
        FirstEvent();
    }
    void SetObjParams()
    {
        objParams.ForEach(eps =>
        {
            var obj = GameObject.Find(eps.objName);
            if (obj == null)
            {
                Debug.LogError(eps.objName + " is not found");
            }
            var roomObj = obj.GetComponent<RoomObj>();
            roomObj.eventParams = eps.eventParams;
            roomObj.eventParams_latter = eps.eventParams_latter;
        });
    }
    abstract protected UniTask FirstEvent();
    public void CheckFlag()
    {
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if (flag >= flagAmount)
        {
            EndEvent().Forget();
        }
    }
    abstract protected UniTask EndEvent();
    protected async UniTask ShowTextEvent(DialogueParams param, Text targetTextUI)
    {
        if (param.timeCount.Length == 0)
        {
            Debug.Log("TextParameterがセットされていません。");
            return;
        }
        for (int i = 0; i < param.dialogues.Length; i++)
        {
            await UniTask.Delay((int)(param.timeCount[0] * 1000), cancellationToken: cts.Token);
            await TextAnim.TypeAnim(targetTextUI, param.dialogues[i], textAppearDuration, cts.Token);
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
                if (Input.GetMouseButtonDown(0)) break;
            }
            //最後のセリフ以外はすぐ消す。
            if (i != param.dialogues.Length - 1) targetTextUI.text = "";
        }
    }
}
