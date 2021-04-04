using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public static class FadeManager
{
    static Color _fadeColor = new Color(0f,0f,0f,1f);

    static float _delay = 0.5f;
    static float _duration = 1f;
    static float _destroyDelay = 0.5f;

    public static void FadeIn()
    {
        var startColor = _fadeColor;
        var endColor = startColor;
        endColor.a = 0;
        FadeTask(startColor, endColor, _delay, _duration, _destroyDelay).Forget();
    }
    public static void FadeOut(){
        var endColor = _fadeColor;
        var startColor = endColor;
        startColor.a = 0;
        FadeTask(startColor, endColor, _delay, _duration, _destroyDelay).Forget();
    }

    static async UniTask FadeTask(Color startColor, Color endColor, float delay, float duration, float destroyDelay){
        //フェード用のCanvas生成
        GameObject fadeCanvasObject = new GameObject("CanvasFade");
        var fadeCanvas = fadeCanvasObject.AddComponent<Canvas>();
        fadeCanvasObject.AddComponent<GraphicRaycaster>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        //最前面になるよう適当なソートオーダー設定
        fadeCanvas.sortingOrder = 100;

        //フェード用のImage生成
        var fadeImage = new GameObject("ImageFade").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchoredPosition = Vector3.zero;

        //Imageサイズは適当に大きく設定してください
        fadeImage.rectTransform.sizeDelta = new Vector2(9999, 9999);

        fadeImage.color = startColor;
        Object.DontDestroyOnLoad(fadeCanvasObject);
        await UniTask.Delay((int)(delay * 1000));
        float t = 0;
        while(t<duration){
            t += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, t / duration);
            await UniTask.Yield();
        }
        await UniTask.Delay((int)(destroyDelay * 1000));
        GameObject.Destroy(fadeCanvasObject);
    }
}
