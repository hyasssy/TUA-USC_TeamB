using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DogDialogueSet
{
    public TextType type;
    [TextArea(1, 4)]
    public string text_ja, text_en;
    public AudioClip clip;
    public float duration = 2f;
    [HideInInspector]
    public string text;
    [HideInInspector]
    public Text targetUI;
}
public abstract class DogPhaseInitializer : PhaseInitializer
{
    [SerializeField]
    float[] eventFlags = default;
    int _currentState = 0;
    [SerializeField]
    AudioSource strokingSoundSource = default;
    [SerializeField]
    AudioClip strokingSoundClip = default;

    protected float strokeSum = 0;
    protected HandController handController;
    //これはHandControllerのIsClickableとやや役割が被っている。
    protected bool stopStroke = false;

    public void StrokingDog()
    {
        if (stopStroke) return;
        var value = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
        if (value != 0)
        {
            // strokeSum += value;
            strokeSum += Time.deltaTime;
            Debug.Log("Stroke dog " + value + ", strokeSum = " + strokeSum);
            StrokeEvent();
            ReactStroke();
        }
    }

    void StrokeEvent()
    {
        if (CheckState() > _currentState)
        {
            _currentState = CheckState();
            EventFire(_currentState).Forget();
        }
    }
    int CheckState()
    {
        float s = strokeSum;
        for (int i = 0; i < eventFlags.Length; i++)
        {
            s -= eventFlags[i];
            if (s < 0)
            {
                return i;
            }
        }
        return eventFlags.Length + 1;
    }
    protected abstract UniTask EventFire(int stateNum);
    //撫でた時のリアクション
    protected void ReactStroke()
    {
        //撫でた音
        LoopSoundOneShot(strokingSoundSource, strokingSoundClip);
        //犬の声を変える
    }
    protected async UniTask ShowTextTask(Text textUI, float duration, string targetText)
    {
        textUI.text = targetText;
        await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        textUI.text = "";
    }
    protected async UniTask EventTask(DogDialogueSet dialogueSet)
    {
        await ShowTextTask(dialogueSet.targetUI, dialogueSet.duration, dialogueSet.text);
    }
    void LoopSoundOneShot(AudioSource audioSource, AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log("Play");
        }
    }

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (targetphase == SetPhase())
        {
            DogMain();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    protected abstract GamePhase SetPhase();
    void DogMain()
    {
        if (strokingSoundSource == default) Debug.LogError("strokingSoundSource is not assigned");
        if (strokingSoundClip == default) Debug.LogError("strokingSouncClip is not assigned");
        FadeManager.FadeIn();
        //クリックできるかを初期化する。
        handController = FindObjectOfType<HandController>();
        handController.SwitchClickable(true);
        stopStroke = false;
        //各シーンごとの初期化
        DogInit();
    }
    abstract protected void DogInit();

}