using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

public abstract class HandController : MonoBehaviour
{
    Vector3 _mousePos;
    RectTransform _canvasRect;
    public Vector2 targetCursorPos = new Vector2(30f, -80f);
    public float handMoveSpeed = 1f;
    [SerializeField]
    Transform hand = default;
    Image _handImage;
    [SerializeField]
    protected Sprite defaultHandSprite = default, hoverHandSprite = default, clickHandSprite = default;
    // RectTransform _canvasRect;
    // protected Vector2 targetCursorPos = new Vector2(30f, -80f);
    // Vector3 _mousePos;
    CommonManager commonManager;
    public bool IsClickable { get; private set; } = true;
    public void SwitchClickable(bool value)
    {
        IsClickable = value;
    }
    bool _isGrabbed = false;
    private void Start()
    {
        commonManager = FindObjectOfType<CommonManager>();
        StartTask().Forget();
    }
    async UniTask StartTask()
    {
        await UniTask.Yield();
        //CommonManagerでHandCanvasがInstantiateされるため、1フレーム遅らせた。
        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            CheckHover();
            CheckClick();
            TracingHand();
        }).AddTo(this);
        if (hand == default) Debug.LogError("hand is not assigned");
        _canvasRect = hand.parent.GetComponent<RectTransform>();
        _handImage = hand.GetComponent<Image>();
        SetUpHand();
    }
    /// <summary>そのシーンのHandImageを差し込む。</summary>
    protected abstract void SetUpHand();
    //hoverしたら画像差し替え
    //クリックしたら画像差し替え
    // bool _isDefaultSize = true;
    string _currentname = null;
    void CheckHover()
    {
        if (!IsClickable) return;
        if (_isGrabbed) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//手から飛ばすのも一考。
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            var coll = hit.collider;
            if (_currentname != coll.transform.name)
            {
                // Debug.Log(coll.gameObject.transform.name);
                _currentname = coll.transform.name;
                var touchable = coll.GetComponent<ITouchable>();
                if (touchable != null)
                {
                    Debug.Log("クリックできるよ：" + coll.transform.name);
                    HoverOn();
                }
                else
                {
                    HoverOff();
                }
            }
        }
    }

    void HoverOn()
    {
        // hand.localScale = Vector3.one * 1.2f;//簡易的に大きくする
        hand.DOScale(Vector3.one * 1.2f, 0.5f);
        HoverOnHandImage();
    }
    void HoverOff()
    {
        hand.DOScale(Vector3.one, 0.5f);//.OnComplete(() => _isDefaultSize = true);//簡易的に戻す
        HoverOffHandImage();
    }
    void HoverOnHandImage()
    {
        ChangeHandImage(hoverHandSprite);
    }
    void HoverOffHandImage()
    {
        ChangeHandImage(defaultHandSprite);
    }

    void CheckClick()
    {
        if (_isGrabbed)
        {
            // Debug.Log("Holding");
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10.0f))
                {
                    var coll = hit.collider;
                    Holding(coll.transform.name);
                }
            }
            else
            {
                ClickOff();
            }
            // if (Input.GetMouseButtonUp(0))
            // {
            //     ClickOff();
            // }
        }
        if (!IsClickable) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f))
            {
                var coll = hit.collider;
                Debug.Log("クリックしたのは" + coll.gameObject.transform.name);
                var touchable = coll.gameObject.GetComponent<ITouchable>();
                if (touchable != null)
                {
                    touchable.Clicked();
                    ClickDown();
                }
                else
                {
                    return;
                }
            }
        }
    }
    void ClickDown()
    {
        _isGrabbed = true;
        ClickOnHandImage();
    }
    protected abstract void Holding(string objName);
    void ClickOff()
    {
        _isGrabbed = false;
        ClickOffHandImage();
    }
    void ClickOnHandImage()
    {
        ChangeHandImage(clickHandSprite);
    }
    void ClickOffHandImage()
    {
        ChangeHandImage(defaultHandSprite);
    }
    protected void ChangeHandImage(Sprite targetSprite)
    {
        _handImage.sprite = targetSprite;
    }
    void TracingHand()
    {
        _mousePos = Input.mousePosition;
        _mousePos.x = Mathf.Clamp(_mousePos.x, 0, Screen.width);
        _mousePos.y = Mathf.Clamp(_mousePos.y, 0, Screen.height);
        var magnification = _canvasRect.sizeDelta.x / Screen.width;
        _mousePos.x = _mousePos.x * magnification - _canvasRect.sizeDelta.x / 2 + targetCursorPos.x;
        _mousePos.y = _mousePos.y * magnification - _canvasRect.sizeDelta.y / 2 + targetCursorPos.y;
        // _mousePos.z = 0;

        hand.localPosition = Vector3.Lerp(hand.localPosition, _mousePos, Time.deltaTime * handMoveSpeed);
    }
}
