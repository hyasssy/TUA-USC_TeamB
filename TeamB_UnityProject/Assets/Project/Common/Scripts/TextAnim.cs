using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System;
using UniRx;
using Cinemachine;
using System.Linq;

public static class TextAnim
{
    public static async UniTask TypeAnim(Text targetUI, string text, float speed, CancellationToken token)
    {
        var letterAmount = text.Length;
        var current = 0;
        string value = "";
        while (current < letterAmount)
        {
            value += text.Substring(current, 1);
            targetUI.text = value;
            current++;
            await UniTask.Delay((int)(speed * 1000), cancellationToken: token);
        }
    }
    public static async UniTask FadeOutText(Graphic targetUI, float duration, CancellationToken token, float delay = 0f)
    {
        await UniTask.Delay((int)(delay * 1000), cancellationToken: token);
        //ホントはキャンセル時にコールバックで透明度を戻す処理を作るのがただしいが…。
        Debug.Log("TextUIの透明度を変更します。ホントはコールバックで戻すべきだが...。");
        var t = 0f;
        var originalColor = targetUI.color;
        Color originalShadowColor = default;
        var shadow = targetUI.gameObject.GetComponent<Shadow>();
        if (shadow == null)
        {
            var originalArpha = originalColor.a;
            while (t < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                t += Time.deltaTime;
                var p = 1f - Easing.QuadInOut(t, duration, 0f, 1f);
                var value = originalColor;
                value.a = originalArpha * p;
                targetUI.color = value;
            }
        }
        else
        {
            var originalArpha = originalColor.a;
            originalShadowColor = shadow.effectColor;
            var originalShadowAlpha = originalShadowColor.a;
            while (t < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                t += Time.deltaTime;
                var p = 1f - Easing.QuadInOut(t, duration, 0f, 1f);
                var value = originalColor;
                value.a = originalArpha * p;
                targetUI.color = value;
                var shadowValue = originalShadowColor;
                shadowValue.a = originalShadowAlpha * p;
                shadow.effectColor = shadowValue;
            }
        }
        var text = targetUI.GetComponent<Text>();
        if (text != null) text.text = "";
        targetUI.color = originalColor;//最後に一応透明度戻しておく。
        if (shadow != null) shadow.effectColor = originalShadowColor;
    }
}
