using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class News0Manager : NewsPhaseInitializer
{
    [Serializable]
    class DialogueSet
    {
        public float[] timeCount;
        public AudioClip[] audioClips;
        [TextArea(1, 4)]
        public string[] dialogues_ja;
        [TextArea(1, 4)]
        public string[] dialogues_en;
    }
    [SerializeField]
    DialogueSet dialogueSet;
    string[] _dialogues;

    [SerializeField]
    float newsDuration = 15f;
    [SerializeField]
    AudioSource newsBGM = default, ambient = default, TVNoise = default;
    Volume _volume;
    Bloom _bloom;
    [SerializeField]
    GameObject newsDisplay = default, filter = default;
    [SerializeField]
    AudioClip quitSound = default;

    override protected void PlaySound()
    {
        //音鳴らす
        newsBGM.enabled = true;
        newsBGM.Play();
        Debug.Log("Play");
        // ambientは通しで流そうかみたいな考えがあった名残。
        // if(!ambient.isPlaying){
        //     ambient.Play();
        // }
    }
    protected override GamePhase SetPhase()
    {
        return GamePhase.News0;
    }

    override protected async UniTask Anim(CancellationToken token)
    {
        //アニメーション制御の要素：newsのscale、bloom、AudioVolume、プチって切り替わって顔が映るトリガー
        var _nap = FindObjectOfType<NewsAnimParam>();
        if (_nap == null)
        {
            Debug.LogAssertion("NewsAnimParamが見つかりません。");
        }
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet<Bloom>(out _bloom);
        var p = _bloom.intensity.value;
        float t = 0f;
        SetDialogues(token).Forget();
        while (t < newsDuration)
        {
            //Set Animating Parameter
            newsBGM.volume = _nap.currentAudioVolume;
            _bloom.intensity.value = _nap.currentBloomIntensity;
            _nap.transform.localScale = new Vector3(_nap.currentScale, _nap.currentScale, 1f);
            t += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
    async UniTask SetDialogues(CancellationToken token)
    {
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        if (lang == Lang.ja)
        {
            _dialogues = dialogueSet.dialogues_ja;
        }
        else
        {
            _dialogues = dialogueSet.dialogues_en;
        }
        var canvas = FindObjectOfType<SubtitleCanvas>();
        var audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("AudioSource is not assigned.");
        for (int i = 0; i < _dialogues.Length; i++)
        {
            await UniTask.Delay((int)(dialogueSet.timeCount[i] * 1000));
            canvas.newsText.text = _dialogues[i];
            audioSource.PlayOneShot(dialogueSet.audioClips[i]);
        }
        await UniTask.Delay((int)(dialogueSet.timeCount[dialogueSet.timeCount.Length - 1] * 1000));
        if (newsDisplay == default || filter == default) Debug.LogError("newsAnim or Filter is not assigned");
        newsDisplay.SetActive(false);
        filter.SetActive(false);
        if (TVNoise == default) Debug.LogError("TVNoise is not assigned");
        TVNoise.Stop();
        if (quitSound == default) Debug.LogError("quitSound is not assigned");
        audioSource.PlayOneShot(quitSound);
        canvas.newsText.text = "";
    }
    override protected void StopSound(CancellationToken token)
    {
        float duration = 1.5f;
        FadeOutSound(newsBGM, duration, token).Forget();
    }
}