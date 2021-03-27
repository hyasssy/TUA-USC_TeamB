using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;

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
    }

    public void Clicked(){
        Debug.Log("クリックされた");

        if(IsClicked){
            return;
        }else{
            IsClicked = true;
        }
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        targetTextPanel.SetActive(true);
        if(dialogues[0] == null) Debug.LogAssertion("There is no text set in the parameter");
        //子の一個めに目的のテキストオブジェがある想定
        _targetText = targetTextPanel.transform.GetChild(0).GetComponent<Text>();
        _targetText.text = "";
        NextText(this.GetCancellationTokenOnDestroy()).Forget();
        //余裕ができたら、なんかアニメーションでインしてくるようにしたり。Cinemachineいじったり。複数回読めるようにprefab化してinstantiateにしたり。
        _disposable = this.UpdateAsObservable()
        .Subscribe(_ => {
            if(Input.GetMouseButtonDown(0)) NextText(this.GetCancellationTokenOnDestroy()).Forget();
        }).AddTo(_targetText);
    }
    async UniTask NextText(CancellationToken token){
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
            return;
        }
        //テキストAnim入れるといい。
        var letterAmount = dialogues[_currentTextNum].Length;
        var current = 1;
        while(current <= letterAmount){
            var s = dialogues[_currentTextNum].Substring(0,current);
            _targetText.text = s;
            current++;
            await UniTask.Delay((int)(typingDuration * 1000), cancellationToken: token);
        }
        _currentTextNum++;
        _isAnimating = false;
    }
}