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
    [SerializeField]
    List<DogDialogueSet> dialogueSets = default;

    SubtitleCanvas _subtitleCanvas;


    //初期化
    protected override void DogInit()
    {
        //BGM鳴らすなどをここで入れる。
        _subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        if (_subtitleCanvas == null) Debug.LogError("subtitleCanvas is not found");
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        dialogueSets.ForEach(d =>
        {
            if (lang == Lang.ja)
            {
                d.text = d.text_ja;
            }
            else
            {
                d.text_ja = d.text_en;
            }
            switch (d.type)
            {
                case TextType.Monologue:
                    d.targetUI = _subtitleCanvas.monologueText;
                    break;
                case TextType.Dialogue:
                    d.targetUI = _subtitleCanvas.dialogueText;
                    break;
                default:
                    Debug.LogError("まだ設定されていない項目です。");
                    break;
            }
        });
    }
    protected override async UniTask EventFire(int stateNum)
    {
        stopStroke = true;
        handController.SwitchClickable(false);
        var num = stateNum - 1;
        switch (stateNum)
        {
            case 1:
                await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
                break;
            case 2:
                await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
                break;
            // case 3:
            //     await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
            //     break;
            // case 4:
            //     await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
            //     break;
            // case 5:
            //     await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
            //     break;
            // case 6:
            //     await ShowTextTask(dialogueSets[num].targetUI, dialogueSets[num].duration, dialogueSets[num].text);
            //     break;
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
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night1);
    }
}