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

        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        await ShowTextEvent(firstTexts, monologueText);
        var duration = 3f;
        _subtitleCanvas.OpenCrop(duration);
        await TextAnim.FadeOutText(monologueText, duration, cts.Token);
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
    }

    protected override async UniTask EndEvent()
    {
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        //何か台詞流したりするならここで。
        // var monologueText = _subtitleCanvas.monologueText;
        // await ShowTextEvent(_firstTexts, monologueText);
        // var duration = 3f;
        // await TextAnim.FadeOutText(monologueText, duration, cts.Token);
        var monologueText = FindObjectOfType<SubtitleCanvas>().monologueText;
        await ShowTextEvent(endTexts, monologueText);
        await UniTask.Delay(1500, cancellationToken: cts.Token);
        TextAnim.FadeOutText(monologueText, 1.4f, cts.Token).Forget();
        FadeOutSound(radio, 1f).Forget();
        FadeOutSound(radioNoise, 1f).Forget();
        FadeOutSound(ambient, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night0);
    }
}