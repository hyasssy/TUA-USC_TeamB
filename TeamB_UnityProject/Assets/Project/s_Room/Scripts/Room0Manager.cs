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
    class DialogueContents
    {
        public float[] timeCount;
        [TextArea(1, 4)]
        public string[] dialogues;
    }
    [SerializeField]
    DialogueContents contents_ja, contents_en;
    DialogueContents _contents;
    [SerializeField]
    float typingDuration = 0.05f;
    protected override GamePhase SetPhase()
    {
        return GamePhase.Room0;
    }
    protected override async UniTask FirstEvent()
    {
        //上下黒クロップいれて映画っぽい演出から始めてもいいな。
        var subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        subtitleCanvas.SetCropPanel();
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        var monologueText = subtitleCanvas.monologueText;
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        _contents = lang == Lang.ja ? contents_ja : contents_en;
        if (_contents.timeCount.Length != _contents.dialogues.Length + 1) Debug.LogWarning("コンテンツの情報が適切にセットされていません。");
        for (int i = 0; i < _contents.dialogues.Length; i++)
        {
            await UniTask.Delay((int)(_contents.timeCount[0] * 1000), cancellationToken: cts.Token);
            await TextAnim.TypeAnim(monologueText, _contents.dialogues[i], typingDuration, cts.Token);
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
                if (Input.GetMouseButtonDown(0)) break;
            }
            if (i != _contents.dialogues.Length - 1) monologueText.text = "";
        }
        var duration = 3f;
        subtitleCanvas.OpenCrop(duration);
        await TextAnim.FadeOutText(monologueText, duration, cts.Token);
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
    }
    protected override void PlaySound()
    {
        radio.Play();
        radioNoise.Play();
        ambient.Play();
    }
    protected override void LoadNextScene()
    {
        FadeOutSound(radio, 1f).Forget();
        FadeOutSound(radioNoise, 1f).Forget();
        FadeOutSound(ambient, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night0);
    }
}