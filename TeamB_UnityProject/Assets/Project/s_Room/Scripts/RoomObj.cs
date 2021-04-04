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

public class RoomObj : MonoBehaviour, ITouchable
{
    [SerializeField,TextArea(1,4)]
    List<string> dialogues_ja = new List<string>();
    [SerializeField,TextArea(1,4)]
    List<string> dialogues_en = new List<string>();
    int _currentTextNum = 0;
    List<string> dialogues;
    [SerializeField]
    GameObject targetTextPanel = default;
    Text _targetText = default;
    public bool IsClicked {get; private set;} = false;
    IDisposable _disposable;
    //setup
    bool _isAnimating = false;
    [SerializeField]
    float typingDuration = 0.02f;
    CancellationTokenSource _cts;

    public void SetUpRoomItem(){
        IsClicked = false;
        _isAnimating = false;
        //再び選択されるのに備えて初期化
        _currentTextNum = 0;
        targetTextPanel.SetActive(false);
        var currentLang = FindObjectOfType<CommonManager>().PlayLang;
        if(currentLang == Lang.ja){
            dialogues = dialogues_ja;
        }else{
            dialogues = dialogues_en;
        }
        if(_cts == null){
            _cts = new CancellationTokenSource();
        }else{
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
    }
    private void OnDisable() {
        if(_cts != null){
            _cts.Cancel();
        }
    }

    public void Clicked(){
        Debug.Log("クリックされた");

        if(IsClicked){
            return;
        }else{
            IsClicked = true;
        }
        ClickedTask().Forget();
    }
    async UniTask ClickedTask(){
        //カメラを切り替える
        //childの1個目にVirtualCameraがある想定
        FindObjectOfType<PlayerCamController>().ChangeCamera(transform.GetChild(0));
        //Wait for camera transition
        var camTransitionTime = FindObjectOfType<CinemachineBrain>().m_DefaultBlend.BlendTime;
        await UniTask.Delay((int)(camTransitionTime * 1000), cancellationToken:_cts.Token);

        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        targetTextPanel.SetActive(true);
        if(dialogues[0] == null) Debug.LogAssertion("There is no text set in the parameter");
        //子の一個めに目的のテキストオブジェがある想定
        _targetText = targetTextPanel.transform.GetChild(0).GetComponent<Text>();
        _targetText.text = "";
        NextText().Forget();
        //余裕ができたら、なんかアニメーションでインしてくるようにしたり。Cinemachineいじったり。複数回読めるようにprefab化してinstantiateにしたり。
        _disposable = this.UpdateAsObservable()
        .Subscribe(_ => {
            if(Input.GetMouseButtonDown(0)) NextText().Forget();
        }).AddTo(_targetText);

    }
    async UniTask NextText(){
        if(_isAnimating){
            return;
        }else{
            _isAnimating = true;
        }
        Debug.Log("NextText");
        if(_currentTextNum >= dialogues.Count){
            //ダイアログ閉じて次いく。
            FindObjectOfType<RoomHandController>().SwitchClickable(true);
            //再び選択されるのに備えて初期化
            _currentTextNum = 0;
            targetTextPanel.SetActive(false);
            FindObjectOfType<RoomPhaseInitializer>().CheckFlag();
            _disposable.Dispose();
            FindObjectOfType<PlayerCamController>().ChangeCamera();
            return;
        }
        //テキストAnim入れるといい。
        var letterAmount = dialogues[_currentTextNum].Length;
        var current = 1;
        while(current <= letterAmount){
            var s = dialogues[_currentTextNum].Substring(0,current);
            _targetText.text = s;
            current++;
            await UniTask.Delay((int)(typingDuration * 1000), cancellationToken: _cts.Token);
        }
        _currentTextNum++;
        _isAnimating = false;
    }
}