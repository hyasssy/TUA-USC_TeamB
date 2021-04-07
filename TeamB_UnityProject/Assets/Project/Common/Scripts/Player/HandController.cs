using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class HandController : MonoBehaviour
{
    Transform _hand;
    HandManager _handManager;
    // RectTransform _canvasRect;
    // protected Vector2 targetCursorPos = new Vector2(30f, -80f);
    // Vector3 _mousePos;
    CommonManager commonManager;
    public bool IsClickable {get; private set; } = true;
    public void SwitchClickable(bool value){
        IsClickable = value;
    }
    private void Start() {
        commonManager = FindObjectOfType<CommonManager>();
        StartTask().Forget();
    }
    async UniTask StartTask(){
        await UniTask.Yield();
        //CommonManagerでHandCanvasがInstantiateされるため、1フレーム遅らせた。
        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            CheckHover();
            Click();
            // TracingHand();
        }).AddTo(this);
        _handManager = FindObjectOfType<HandManager>();
        // _canvasRect = _handManager.GetComponent<RectTransform>();
        _hand = _handManager.transform.GetChild(0);
        SetUpHand();
    }
    protected abstract void SetUpHand();
    //hoverしたら画像差し替え
    //クリックしたら画像差し替え
    // bool _isDefaultSize = true;
    string _currentname = null;
    void CheckHover(){
        if(!IsClickable) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//手から飛ばすのも一考。
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,10.0f))
        {
            var coll = hit.collider;
            if(_currentname != coll.gameObject.transform.name){
                Debug.Log(coll.gameObject.transform.name);
                _currentname = coll.gameObject.transform.name;
                var touchable = coll.gameObject.GetComponent<ITouchable>();
                if(touchable != null){
                    Debug.Log("クリックできるよ");
                    // hand.localScale = Vector3.one * 1.2f;//簡易的に大きくする
                    _hand.DOScale(Vector3.one * 1.2f, 0.5f);
                }else{
                    _hand.DOScale(Vector3.one, 0.5f);//.OnComplete(() => _isDefaultSize = true);//簡易的に戻す
                }
            }
        }
    }
    void Click(){
        if(!IsClickable) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                var coll = hit.collider;
                Debug.Log(coll.gameObject.transform.name);
                var touchable = coll.gameObject.GetComponent<ITouchable>();
                if (touchable != null)
                {
                    touchable.Clicked();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
