using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using Cinemachine;
using System.Linq;

public enum TextType
{
    Monologue,
    Dialogue,
    News,
    Narration,
    Special
}
public abstract class RoomObj : MonoBehaviour, ITouchable
{
    [field: SerializeField, RenameField(nameof(IsImportant))]
    public bool IsImportant { get; private set; } = true;
    [Serializable]
    protected class EventParamSet
    {
        public TextType type;
        [TextArea(1, 4)]
        public string text_ja, text_en;
        [HideInInspector]
        public string text;
        [HideInInspector]
        public Text targetUI;
        [HideInInspector]
        public float speed;

    }
    [SerializeField]
    protected List<EventParamSet> eventParams;
    [SerializeField]
    List<EventParamSet> eventParams_latter = default;
    protected int currentEvent = 0;

    [SerializeField]
    GameObject targetTextPanelObj = default;
    protected bool isClicked = false;
    IDisposable _disposable;
    bool _onTask = false;
    protected CancellationTokenSource cts;
    AudioSource objSound;

    public void SetUpRoomItem()
    {
        isClicked = false;
        _onTask = false;
        //選択されるのに備えて初期化
        currentEvent = 0;
        targetTextPanelObj.SetActive(false);
        InitEventParams();

        if (cts == null)
        {
            cts = new CancellationTokenSource();
        }
        else
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
        }
    }
    void InitEventParams()
    {
        var param = Resources.Load<SetParam>("SetGameParam");
        //子の一個めに目的のテキストオブジェがある想定
        var targetTextPanel = targetTextPanelObj.transform.GetChild(0).GetComponent<Text>();
        var currentLang = FindObjectOfType<CommonManager>().PlayLang;
        var subtitleCanvas = FindObjectOfType<SubtitleCanvas>();
        eventParams.ForEach(p =>
        {
            p.text = currentLang == Lang.ja ? p.text_ja : p.text_en;
            switch (p.type)
            {
                case TextType.Monologue:
                    p.targetUI = subtitleCanvas.monologueText;
                    p.speed = param.VoiceTypingSpeed;
                    break;
                case TextType.Dialogue:
                    p.targetUI = subtitleCanvas.dialogueText;
                    p.speed = param.VoiceTypingSpeed;
                    break;
                case TextType.News:
                    p.targetUI = subtitleCanvas.newsText;
                    break;
                case TextType.Narration:
                    p.targetUI = subtitleCanvas.narrationText;
                    p.speed = param.TextTypingSpeed;
                    break;
                case TextType.Special:
                    p.targetUI = targetTextPanel;
                    p.speed = param.TextTypingSpeed;
                    break;
                default: break;
            }
        });
    }
    private void OnDisable()
    {
        if (cts != null)
        {
            cts.Cancel();
        }
    }

    public void Clicked()
    {
        Debug.Log("クリックされた");
        ClickedTask().Forget();
    }
    async UniTask ClickedTask()
    {
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        // カメラを切り替える
        // 子オブジェクトの中にVirtualCameraをおく。
        var targetVirtualCamera = transform.GetComponentsInChildren<CinemachineVirtualCamera>(true)[0].transform;
        if (targetVirtualCamera == null) Debug.LogAssertion("VirtualCameraが見つかりません。");
        await FindObjectOfType<PlayerCamController>().ChangeCamera(targetVirtualCamera);
        // await UniTask.Delay((int)(camTransitionTime * 1000), cancellationToken:_cts.Token);
        // targetTextPanelObj.SetActive(true);
        if (eventParams[0].text == null) Debug.LogError("There is no text set in the parameter");
        NextTask().Forget();//subscribeの問題があるため
        objSound = GetComponent<AudioSource>();
        if (objSound == null)
        {
            Debug.LogWarning("RoomObjにサウンドが設定されていません。");
        }
        else
        {
            objSound.Play();
        }
        //余裕ができたら、なんかアニメーションでインしてくるようにしたり。Cinemachineいじったり。複数回読めるようにprefab化してinstantiateにしたり。
        _disposable = this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            if (Input.GetMouseButtonDown(0)) NextTask().Forget();
        }).AddTo(gameObject);
    }

    async UniTask NextTask()
    {
        // Debug.Log("NextTask");
        if (_onTask) return;
        _onTask = true;

        await Next();
        // await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        _onTask = false;
    }
    protected abstract UniTask Next();

    //text
    protected async UniTask NextText()
    {
        Debug.Log("NextText" + eventParams[currentEvent].targetUI.gameObject);
        if (eventParams[currentEvent].type == TextType.Special) targetTextPanelObj.SetActive(true);
        await TextAnim.TypeAnim(eventParams[currentEvent].targetUI, eventParams[currentEvent].text, eventParams[currentEvent].speed, cts.Token);
        currentEvent++;
    }
    protected async UniTask EndDialogue()
    {
        Debug.Log("EndDialogue");
        //ダイアログ閉じて次いく。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //再び選択されるのに備えて初期化
        currentEvent = 0;
        targetTextPanelObj.SetActive(false);
        if (eventParams_latter.Count != 0)
        {
            //上書きする。
            eventParams = eventParams_latter;
            InitEventParams();
            Debug.Log("eventParamsを選択後のものに変更しました。");
        }
        if (!isClicked)//クリック判定は一度だけ。
        {
            isClicked = true;
            if (IsImportant) FindObjectOfType<RoomPhaseInitializer>().CheckFlag();
        }
        _disposable.Dispose();
        FindObjectOfType<SubtitleCanvas>().SetUpTexts();
        FindObjectOfType<PlayerCamController>().ChangeCamera().Forget();
        await UniTask.Yield();
    }
}