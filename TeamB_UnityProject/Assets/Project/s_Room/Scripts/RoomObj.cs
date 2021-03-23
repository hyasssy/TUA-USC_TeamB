using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
 
public class RoomObj : MonoBehaviour, ITouchable
{
    [SerializeField,TextArea(1,4)]
    List<string> dialogues = new List<string>();
    int _currentTextNum = 0;
    [SerializeField]
    GameObject targetTextPanel = default;
    Text _targetText = default;

    public void Clicked(){
        Debug.Log("クリックされた");

        if(targetTextPanel == null) return;
        FindObjectOfType<CommonManager>().SwitchClickable(false);
        targetTextPanel.SetActive(true);
        //子の一個めに目的のテキストがある想定
        _targetText = targetTextPanel.transform.GetChild(0).GetComponent<Text>();
        _targetText.text = dialogues[0];
        //余裕ができたら、なんかアニメーションでインしてくるようにしたり。Cinemachineいじったり。複数回読めるようにprefab化してinstantiateにしたり。
        this.UpdateAsObservable()
        .Subscribe(_ => {
            if(Input.GetMouseButtonDown(0)) NextText();
        }).AddTo(targetTextPanel);
    }
    void NextText(){
        Debug.Log("NextText");
        if(_currentTextNum + 1 >= dialogues.Count){
            //ダイアログ閉じて次いく。
            FindObjectOfType<CommonManager>().SwitchClickable(true);
            //再び選択されるのに備えて初期化
            _currentTextNum = 0;
            Destroy(targetTextPanel);
            FindObjectOfType<PhaseInitializer>().CheckFlag();
            return;
        }
        _currentTextNum++;
        _targetText.text = dialogues[_currentTextNum];
    }
}