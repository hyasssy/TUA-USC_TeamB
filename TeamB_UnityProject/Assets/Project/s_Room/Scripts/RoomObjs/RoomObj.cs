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

public abstract class RoomObj : MonoBehaviour, ITouchable
{
    [field: SerializeField, RenameField(nameof(IsImportant))]
    public bool IsImportant { get; private set; } = true;
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_ja = new List<string>();
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_en = new List<string>();
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_latter_ja = new List<string>();
    [SerializeField, TextArea(1, 4)]
    List<string> dialogues_latter_en = new List<string>();
    protected int currentTextNum = 0;
    protected List<string> dialogues;

    [SerializeField]
    GameObject targetTextPanel = default;
    Text _targetText = default;
    protected bool isClicked = false;
    IDisposable _disposable;
    //setup
    bool _onTask = false;
    // [SerializeField]
    float typingDuration = 0.05f;
    CancellationTokenSource _cts;
    AudioSource objSound;

    public void SetUpRoomItem()
    {
        isClicked = false;
        _onTask = false;
        //選択されるのに備えて初期化
        currentTextNum = 0;
        targetTextPanel.SetActive(false);
        var currentLang = FindObjectOfType<CommonManager>().PlayLang;
        if (currentLang == Lang.ja)
        {
            dialogues = dialogues_ja;
        }
        else
        {
            dialogues = dialogues_en;
        }
        if (_cts == null)
        {
            _cts = new CancellationTokenSource();
        }
        else
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
    }
    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
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
        targetTextPanel.SetActive(true);
        if (dialogues[0] == null) Debug.LogAssertion("There is no text set in the parameter");
        //子の一個めに目的のテキストオブジェがある想定
        _targetText = targetTextPanel.transform.GetChild(0).GetComponent<Text>();
        _targetText.text = "";
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
        }).AddTo(_targetText);

    }

    protected abstract UniTask Next(CancellationToken token);
    async UniTask NextTask()
    {
        Debug.Log("NextTask");
        if (_onTask)
        {
            return;
        }
        else
        {
            _onTask = true;
        }
        await Next(_cts.Token);
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: _cts.Token);
        _onTask = false;
    }

    //NextTextを分解して、各種処理を挟めるようにする。
    protected async UniTask NextText()
    {
        Debug.Log("NextText");
        //テキストAnim入れるといい。
        var letterAmount = dialogues[currentTextNum].Length;
        var current = 1;
        while (current <= letterAmount)
        {
            var s = dialogues[currentTextNum].Substring(0, current);
            _targetText.text = s;
            current++;
            await UniTask.Delay((int)(typingDuration * 1000), cancellationToken: _cts.Token);
        }
        currentTextNum++;
    }
    protected async UniTask EndDialogue()
    {
        Debug.Log("EndDialogue");
        //ダイアログ閉じて次いく。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //再び選択されるのに備えて初期化
        currentTextNum = 0;
        var currentLang = FindObjectOfType<CommonManager>().PlayLang;
        if (currentLang == Lang.ja)
        {
            if (dialogues_latter_ja.Count != 0)
            {
                dialogues = dialogues_latter_ja;
                Debug.Log("dialoguesを選択後の文章に変更しました。");
            }
        }
        else
        {
            if (dialogues_latter_en.Count != 0)
            {
                dialogues = dialogues_latter_en;
                Debug.Log("dialoguesを選択後の文章に変更しました。");
            }
        }
        targetTextPanel.SetActive(false);
        if (!isClicked)
        {
            isClicked = true;
            FindObjectOfType<RoomPhaseInitializer>().CheckFlag();
        }
        _disposable.Dispose();
        await FindObjectOfType<PlayerCamController>().ChangeCamera();
    }
}