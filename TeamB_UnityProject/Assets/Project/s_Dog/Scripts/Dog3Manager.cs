using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Dog3Manager : DogPhaseInitializer
{
    [Serializable]
    class DialogueSet
    {
        public string[] dialogues = default;
        public float[] dialoguesDuration = default;
    }
    [SerializeField]
    DialogueSet dialogueSet_ja = default, dialogueSet_en = default;
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
    protected override async UniTask EventFire(int stateNum)
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
                LoadNextScene();
                break;
        }
        stopStroke = false;
        handController.SwitchClickable(true);
    }
    void LoadNextScene()
    {
        // FadeOutSound(radio, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Ending);
    }
}