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



public class EndingManager : PhaseInitializer
{

    [SerializeField]
    float initialDelay = 1f, slideSpeed = 1f;
    [SerializeField]
    RectTransform credit_ja = default, credit_en = default;
    RectTransform _credit;
    [SerializeField]
    GameObject credit1_ja, credit1_en;
    protected override void InitializePhase(GamePhase targetphase)
    {
        switch (targetphase)
        {
            case GamePhase.Ending:
                Ending().Forget();
                break;
            case GamePhase.Ending_1:
                Ending_1().Forget();
                break;
            default:
                Debug.LogError("phase移行がうまくできていません。Error");
                break;

        }
    }
    async UniTask Ending_1()
    {
        await UniTask.Yield();
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        credit1_ja.gameObject.SetActive(lang == Lang.ja);
        credit1_en.gameObject.SetActive(lang != Lang.ja);
    }
    async UniTask Ending()
    {
        await UniTask.Yield();
        var lang = FindObjectOfType<CommonManager>().PlayLang;
        credit_ja.gameObject.SetActive(lang == Lang.ja);
        credit_en.gameObject.SetActive(lang != Lang.ja);
        _credit = lang == Lang.ja ? credit_ja : credit_en;
        // なんか絵をだしたりアニメーションしたりするならここで制御。
        await SlideText(initialDelay, slideSpeed);
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        return;
    }
    public async UniTask SlideText(float delay, float speed)
    {
        var creditSize = _credit.sizeDelta;
        float l = 0;
        creditSize.y = l;
        _credit.sizeDelta = creditSize;

        await UniTask.Delay((int)(delay * 1000), cancellationToken: cts.Token);
        //heightを上げていく。
        var length = _credit.GetComponent<CreditRoll>().length;
        while (l < length)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
            l += Time.deltaTime * speed;
            creditSize.y = l;
            _credit.sizeDelta = creditSize;
        }
        var startValue = l;
        var duration = 6f;
        //endをスライドしていく
        DOTween.To(() => startValue, (val) =>
        {
            l = val;
            creditSize.y = l;
            _credit.sizeDelta = creditSize;
        }, startValue + 540f, duration).SetEase(Ease.OutQuad);
    }
}
