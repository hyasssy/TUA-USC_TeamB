using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Dog1Manager : DogPhaseInitializer
{
    // [SerializeField]
    // AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    [SerializeField]
    float[] eventFlags;
    int _currentState = 0;
    [Serializable]
    class DialogueSet
    {
        [TextArea(1, 4)]
        public string[] dialogues;
        public float[] dialoguesDuration;
    }
    [SerializeField]
    DialogueSet dialogueSet_ja, dialogueSet_en;
    DialogueSet _dialogueSet;
    [SerializeField]
    float[] dialoguesDuration;
    SubtitleCanvas _subtitleCanvas;


    //初期化
    protected override void DogInit()
    {
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        _dialogueSet = lang == Lang.ja ? dialogueSet_ja : dialogueSet_en;
        //BGM鳴らすなどをここで入れる。
        _subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        if (_subtitleCanvas == null) Debug.LogError("subtitleCanvas is not found");
    }
    //UniTaskで反応検知して完結するイベントを作る。
    protected override void StrokeEvent()
    {
        if (CheckState() > _currentState)
        {
            _currentState = CheckState();
            EventFire(_currentState).Forget();
        }
    }
    protected override void ReactStroke()
    {
        //撫でている間は犬の吐息のタイプを変更する。
    }
    int CheckState()
    {
        float s = strokeSum;
        for (int i = 0; i < eventFlags.Length; i++)
        {
            s -= eventFlags[i];
            if (s < 0)
            {
                return i;
            }
        }
        return eventFlags.Length + 1;
    }
    async UniTask EventFire(int stateNum)
    {
        stopStroke = true;
        handController.SwitchClickable(false);
        var num = stateNum - 1;
        switch (stateNum)
        {
            case 1:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            case 2:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            case 3:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            case 4:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            case 5:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            case 6:
                await ShowTextTask(_subtitleCanvas.monologueText, _dialogueSet.dialoguesDuration[num], _dialogueSet.dialogues[num]);
                break;
            default:
                Debug.LogWarning("Eventが設定されてないよ。");
                break;
        }
        stopStroke = false;
        handController.SwitchClickable(true);
    }
    async UniTask ShowTextTask(Text textUI, float duration, string targetText)
    {
        textUI.text = targetText;
        await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        textUI.text = "";
    }


    void LoadNextScene()
    {
        // FadeOutSound(radio, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night1);
    }
}