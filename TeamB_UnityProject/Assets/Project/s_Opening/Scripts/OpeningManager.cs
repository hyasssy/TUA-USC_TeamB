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
    [SerializeField]
    GameObject settingGroup = default;
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
        if (toggle_ja.isOn)
        {
            commonManager.ChangeLang(Lang.ja);
        }
        else
        {
            commonManager.ChangeLang(Lang.en);
        }
        //テキストをフェードアウトさせ、タイトル出すアニメ。
        settingGroup.SetActive(false);
        Opening().Forget();
    }

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (settingGroup == default) Debug.LogWarning("settingGroup is not assigned.");
        if (startButton == default) Debug.LogWarning("startButton is not assigned.");
        if (toggle_ja == default || toggle_en == default) Debug.LogWarning("toggle_ja or toggle_en is not assigned.");

        if (targetphase == GamePhase.Opening)
        {
            // Opening().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    async UniTask Opening()
    {
        await UniTask.Delay(2000);
        var sc = FindObjectOfType<SubtitleCanvas>();
        var narration = sc.narrationText;
        narration.text = "Fragile";
        await UniTask.Delay(3500);
        narration.text = "";
        await UniTask.Delay(1500);
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.News0);
    }
}
