using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

public abstract class NightPhaseInitializer : PhaseInitializer
{
    CancellationTokenSource _cts;
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_ja = new List<string>();
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_en = new List<string>();
    List<string> _dialogues;
    [SerializeField]
    float initialDelay = 2.5f, endDelay = 0.5f;
    [SerializeField]
    float typingDuration = 0.04f;
    Text _targetText;

    public override void InitializePhase(GamePhase targetphase)
    {
        if (_cts == null)
        {
            _cts = new CancellationTokenSource();
        }
        else
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
        Debug.Log("InitializePhase");
        if (targetphase == SetPhase())
        {
            Night().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    protected abstract GamePhase SetPhase();

    private void OnDestroy()
    {
        _cts.Cancel();
    }

    async UniTask Night()
    {
        await UniTask.Delay((int)(initialDelay * 1000), cancellationToken: _cts.Token);
        _targetText = FindObjectOfType<SubtitleCanvas>().narrationText;
        var currentLang = FindObjectOfType<CommonManager>().PlayLang;
        _dialogues = currentLang == Lang.ja ? dialogues_ja : dialogues_en;
        for (int i = 0; i < _dialogues.Count; i++)
        {
            var letterAmount = _dialogues[i].Length;
            var current = 1;
            while (current <= letterAmount)
            {
                var s = _dialogues[i].Substring(0, current);
                _targetText.text = s;
                current++;
                await UniTask.Delay((int)(typingDuration * 1000), cancellationToken: _cts.Token);
            }
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
                if (Input.GetMouseButtonDown(0)) break;
            }
        }
        //テキスト消す
        await UniTask.Delay((int)(endDelay * 1000), cancellationToken: _cts.Token);

        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Room1);
    }

}