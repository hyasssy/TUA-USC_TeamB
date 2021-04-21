using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public class Dog3Manager : DogPhaseInitializer
{
    enum TargetTextPanelType
    {
        Kanie,
        Sumire
    }
    [Serializable]
    class DialogueSet
    {
        public TargetTextPanelType targetTextPanelType;
        public bool isSound = true;
        [HideInInspector]
        public Text targetTextPanel;
        public string[] dialogue_ja = default;
        public string[] dialogues_en = default;
        [HideInInspector]
        public string[] dialogue = default;
        public float dialoguesDuration = default;
    }

    [SerializeField]
    List<DialogueSet> dialogueSets = default;
    SubtitleCanvas _subtitleCanvas;
    [SerializeField]
    Renderer sumireFaceImage = default;
    [SerializeField]
    AudioSource bgm, bgm_withDrum, voice;
    [SerializeField]
    int switchTiming = 3;
    [SerializeField]
    AudioClip[] kanieVoices, sumireVoices;

    //初期化
    protected override void DogInit()
    {
        _subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        if (_subtitleCanvas == null) Debug.LogError("subtitleCanvas is not found");
        if (dialogueSets == default) Debug.LogWarning("dialogueSets is not assigned");
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        dialogueSets.ForEach(d =>
        {
            switch (d.targetTextPanelType)
            {
                case TargetTextPanelType.Kanie:
                    d.targetTextPanel = _subtitleCanvas.monologueText;
                    break;
                case TargetTextPanelType.Sumire:
                    d.targetTextPanel = _subtitleCanvas.dialogueText;
                    break;
                default:
                    Debug.LogWarning("Something wrong has occurred");
                    break;
            }
            d.dialogue = lang == Lang.ja ? d.dialogue_ja : d.dialogues_en;
        });
        //BGM鳴らすなどをここで入れる。
    }
    protected override async UniTask EventFire(int stateNum)
    {
        stopStroke = true;
        handController.SwitchClickable(false);
        var num = stateNum - 1;
        if (num == switchTiming)
        {
            ImageFadeIn(sumireFaceImage).Forget();
            Switchsound().Forget();
        }
        if (num > dialogueSets.Count)
        {
            LoadNextScene();
        }
        else
        {
            if (dialogueSets[num].isSound)
            {
                //声流す
                if (dialogueSets[num].targetTextPanelType == TargetTextPanelType.Kanie)
                {
                    voice.PlayOneShot(kanieVoices[UnityEngine.Random.Range(0, kanieVoices.Length)]);
                }
                else
                {
                    voice.PlayOneShot(sumireVoices[UnityEngine.Random.Range(0, sumireVoices.Length)]);
                }
            }
            foreach (string d in dialogueSets[num].dialogue)
            {
                await ShowTextTask(dialogueSets[num].targetTextPanel, dialogueSets[num].dialoguesDuration, d);
            }
        }

        stopStroke = false;
        handController.SwitchClickable(true);
    }
    protected override GamePhase SetPhase()
    {
        return GamePhase.Dog3;
    }
    void LoadNextScene()
    {
        // FadeOutSound(radio, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Ending_1);
    }
    async UniTask ImageFadeIn(Renderer renderer)
    {
        var duration = 2f;
        var color = renderer.material.color;
        DOTween.To(() => 0f, (val) =>
        {
            color.a = val;
            renderer.material.SetColor("_BaseColor", color);
        }, 0.95f, duration);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
    }
    async UniTask Switchsound()
    {
        var t = 0f;
        var duration = 2f;
        var bgm_originalVolume = bgm.volume;
        while (t < duration)
        {
            await UniTask.Yield();
            t += Time.deltaTime;
            bgm.volume = 1 - (bgm_originalVolume * t / duration);
            bgm_withDrum.volume = t / duration;
        }
        bgm.gameObject.SetActive(false);
    }
}