using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class HandController : MonoBehaviour
{
    [SerializeField]
    Transform hand = default;
    [SerializeField]
    RectTransform canvasRect = default;
    [SerializeField]
    Vector2 targetCursorPos = new Vector2(30f, -80f);
    Vector3 _mousePos;
    public bool isClickable = true;
    private void Start() {
        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            CheckHover();
            Click();
            TracingHand();
        });
    }
    //hoverしたら画像差し替え
    //クリックしたら画像差し替え

    void CheckHover(){
        if(!isClickable) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//手から飛ばすのも一考。
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,10.0f))
        {
            var coll = hit.collider;
            Debug.Log(coll.gameObject.transform.name);
            var touchable = coll.gameObject.GetComponent<ITouchable>();
            if(touchable != null){
                Debug.Log("クリックできるよ");
                hand.localScale = Vector3.one * 1.2f;//簡易的に大きくする
            }else{
                hand.localScale = Vector3.one;//簡易的に戻す
            }
        }
    }
    void Click(){
        if(!isClickable) return;
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