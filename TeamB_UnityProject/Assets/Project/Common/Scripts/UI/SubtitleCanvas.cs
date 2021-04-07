using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

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

    private void Start()
    {
        SetUpText(monologueText);
        SetUpText(newsText);
        SetUpText(dialogueText);
        SetUpText(narrationText);
        _cts = new CancellationTokenSource();
    }
    void SetUpText(Text t)
    {
        if (t == default)
        {
            Debug.LogError(nameof(t) + " is not assigned");
        }
        t.text = "";
    }
    public void ShowMonologue(string text, float showDuration)
    {
        DisplayText(monologueText, text, showDuration).Forget();
    }
    public void ShowNews(string text, float showDuration)
    {
        DisplayText(newsText, text, showDuration).Forget();
    }
    public void ShowDialogue(string text, float showDuration)
    {
        DisplayText(dialogueText, text, showDuration).Forget();
    }
    public void ShowNarration(string text, float showDuration)
    {
        DisplayText(narrationText, text, showDuration).Forget();
    }
    async UniTask DisplayText(Text targetText, string text, float showDuration)
    {
        //数秒待って消す。
        //必要があれば、引数からenumタイプ指定する形でアニメーション出してもいいか。
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        targetText.text = text;
        await UniTask.Delay((int)(showDuration * 1000), cancellationToken: _cts.Token);
        targetText.text = "";
    }
}
