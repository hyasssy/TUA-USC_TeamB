using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class RoomHandController : MonoBehaviour
{
    [SerializeField]
    Transform hand = default;
    [SerializeField]
    RectTransform canvasRect = default;
    [SerializeField]
    Vector2 targetCursorPos = new Vector2(30f, -80f);
    Vector3 _mousePos;
    CommonManager commonManager;
    public bool IsClickable {get; private set; } = true;
    public void SwitchClickable(bool value){
        IsClickable = value;
    }
    private void Start() {
        commonManager = FindObjectOfType<CommonManager>();
        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            CheckHover();
            Click();
            TracingHand();
        }).AddTo(this);
    }
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
                    hand.DOScale(Vector3.one * 1.2f, 0.5f);
                }else{
                    hand.DOScale(Vector3.one, 0.5f);//.OnComplete(() => _isDefaultSize = true);//簡易的に戻す
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
    void TracingHand(){
        _mousePos = Input.mousePosition;
        var magnification = canvasRect.sizeDelta.x / Screen.width;
        _mousePos.x = _mousePos.x * magnification - canvasRect.sizeDelta.x / 2 + targetCursorPos.x;
        _mousePos.y = _mousePos.y * magnification - canvasRect.sizeDelta.y / 2 + targetCursorPos.y;
        _mousePos.z = transform.localPosition.z;

        hand.localPosition = Vector3.Lerp(hand.localPosition, _mousePos, Time.deltaTime);
    }
}