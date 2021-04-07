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
    [SerializeField]
    Text monologueText = default, narrateText = default;

    private void Start() {
        if(monologueText == default){
            Debug.LogError("MonologueText is not assigned");
        }
        monologueText.text = "";
        if(narrateText == default){
            Debug.LogError("NarrateText is not assigned");
        }
        narrateText.text = "";
        _cts = new CancellationTokenSource();
    }
    public void ShowMonologue(string text, float showDuration){
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        ShowMonologueTask(monologueText, text, showDuration).Forget();
    }
    public void ShowNarration(string text, float showDuration){
        ShowMonologueTask(narrateText, text, showDuration).Forget();
    }
    async UniTask ShowMonologueTask(Text targetText, string text, float showDuration){
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        targetText.text = text;
        await UniTask.Delay((int)(showDuration * 1000), cancellationToken: _cts.Token);
        targetText.text = "";
    }
}
