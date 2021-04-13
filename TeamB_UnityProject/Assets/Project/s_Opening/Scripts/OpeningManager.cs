using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.UI;



public class OpeningManager : PhaseInitializer
{
    //言語選択
    public bool IsJapanese { get; private set; } = true;
    [SerializeField]
    Toggle toggle_ja = default, toggle_en = default;
    [SerializeField]
    Button startButton = default;

    public void StartGame()
    {
        startButton.interactable = false;
        toggle_ja.interactable = false;
        toggle_en.interactable = false;
        var commonManager = FindObjectOfType<CommonManager>();
        if (toggle_ja)
        {
            commonManager.ChangeLang(Lang.ja);
        }
        else
        {
            commonManager.ChangeLang(Lang.en);
        }
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.News0);
    }

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (startButton == default) Debug.LogWarning("startButton is not assigned.");
        if (toggle_ja == default || toggle_en == default) Debug.LogWarning("toggle_ja or toggle_en is not assigned.");

        Debug.Log("InitializePhase");
        if (targetphase == GamePhase.Opening)
        {
            Opening().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    async UniTask Opening()
    {
        // なんか絵をだしたりアニメーションしたりするならここで制御。
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        return;
    }
}
