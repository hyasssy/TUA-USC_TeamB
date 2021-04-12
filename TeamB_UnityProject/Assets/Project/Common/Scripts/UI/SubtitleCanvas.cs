using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using DG.Tweening;

public class SubtitleCanvas : MonoBehaviour
{
    CancellationTokenSource _cts;
    [field: SerializeField, RenameField(nameof(monologueText))]
    public Text monologueText { get; private set; } = default;
    [field: SerializeField, RenameField(nameof(newsText))]
    public Text newsText { get; private set; } = default;
    [field: SerializeField, RenameField(nameof(dialogueText))]
    public Text dialogueText { get; private set; } = default;
    [field: SerializeField, RenameField(nameof(narrationText))]
    public Text narrationText { get; private set; } = default;
    [SerializeField]
    RectTransform cropPanelTop = default, cropPanelBottom = default;
    Vector2 _startCropSize;
    float _textTypingSpeed;
    float _voiceTypingSpeed;

    private void Start()
    {
        var param = Resources.Load<SetParam>("SetGameParam");
        _textTypingSpeed = param.TextTypingSpeed;
        _voiceTypingSpeed = param.VoiceTypingSpeed;
        SetUpTexts();
        _cts = new CancellationTokenSource();
        _startCropSize = cropPanelBottom.sizeDelta;
    }
    void EraseText(Text t)
    {
        if (t == default)
        {
            Debug.LogError(nameof(t) + " is not assigned");
        }
        t.text = "";
    }
    public void SetUpTexts()
    {
        EraseText(monologueText);
        EraseText(newsText);
        EraseText(dialogueText);
        EraseText(narrationText);
    }
    // public void ShowMono
    async UniTask DisplayText(Text targetText, string text, float showDuration, bool isType, bool isVoiceSpeed = true)
    {
        //数秒待って消す。
        //必要があれば、引数からenumタイプ指定する形でアニメーション出してもいいか。
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        if (isType)
        {
            var speed = isVoiceSpeed ? _voiceTypingSpeed : _textTypingSpeed;
            await TextAnim.TypeAnim(targetText, text, speed, _cts.Token);
        }
        else
        {
            targetText.text = text;
        }
        await UniTask.Delay((int)(showDuration * 1000), cancellationToken: _cts.Token);
        targetText.text = "";
    }
    public void SetCropPanel()
    {
        // cropPanelTop.sizeDelta = _startCropSize;
        // cropPanelBottom.sizeDelta = _startCropSize;
        cropPanelTop.gameObject.SetActive(true);
        cropPanelBottom.gameObject.SetActive(true);
    }
    public void OpenCrop(float duration)
    {
        DOTween.To(() => _startCropSize.y, (val) =>
        {
            var value = _startCropSize;
            value.y = val;
            cropPanelTop.sizeDelta = value;
            cropPanelBottom.sizeDelta = value;
        }, 0, duration).SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            cropPanelTop.gameObject.SetActive(false);
            cropPanelBottom.gameObject.SetActive(false);
            cropPanelTop.sizeDelta = _startCropSize;
            cropPanelBottom.sizeDelta = _startCropSize;
        });
    }
}
