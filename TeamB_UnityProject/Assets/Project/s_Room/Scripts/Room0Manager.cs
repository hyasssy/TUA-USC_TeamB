using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System;

public class Room0Manager : RoomPhaseInitializer
{
    [SerializeField]
    AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    // [SerializeField]
    // AudioClip mainSound = default;
    [Serializable]
    class DialogueParams
    {
        public float[] timeCount = default;
        [TextArea(1, 4)]
        public string[] dialogues = default;
    }
    [SerializeField]
    DialogueParams firstTexts_ja = default, firstTexts_en = default;
    DialogueParams _firstTexts;
    [SerializeField]
    DialogueParams endTexts_ja = default, endTexts_en = default;
    DialogueParams _endTexts;
    [SerializeField]
    float typingDuration = 0.05f;
    SubtitleCanvas _subtitleCanvas;
    protected override GamePhase SetPhase()
    {
        return GamePhase.Room0;
    }
    protected override async UniTask FirstEvent()
    {
        //上下黒クロップいれて映画っぽい演出から始めてもいいな。
        _subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        _subtitleCanvas.SetCropPanel();
        var monologueText = _subtitleCanvas.monologueText;
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        _firstTexts = lang == Lang.ja ? firstTexts_ja : firstTexts_en;
        _endTexts = lang == Lang.ja ? endTexts_ja : endTexts_en;

        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        await ShowTextEvent(_firstTexts, monologueText);
        var duration = 3f;
        _subtitleCanvas.OpenCrop(duration);
        await TextAnim.FadeOutText(monologueText, duration, cts.Token);
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
    }
    async UniTask ShowTextEvent(DialogueParams param, Text targetTextUI)
    {
        if (param.timeCount.Length != param.dialogues.Length + 1) Debug.LogWarning("コンテンツの情報が適切にセットされていません。");
        for (int i = 0; i < param.dialogues.Length; i++)
        {
            await UniTask.Delay((int)(param.timeCount[0] * 1000), cancellationToken: cts.Token);
            await TextAnim.TypeAnim(targetTextUI, param.dialogues[i], typingDuration, cts.Token);
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
                if (Input.GetMouseButtonDown(0)) break;
            }
            //最後のセリフ以外はすぐ消す。
            if (i != param.dialogues.Length - 1) targetTextUI.text = "";
        }
    }
    protected override async UniTask EndEvent()
    {
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        //何か台詞流したりするならここで。
        // var monologueText = _subtitleCanvas.monologueText;
        // await ShowTextEvent(_firstTexts, monologueText);
        // var duration = 3f;
        // await TextAnim.FadeOutText(monologueText, duration, cts.Token);

        FadeOutSound(radio, 1f).Forget();
        FadeOutSound(radioNoise, 1f).Forget();
        FadeOutSound(ambient, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night0);
        await UniTask.Yield();
    }
}