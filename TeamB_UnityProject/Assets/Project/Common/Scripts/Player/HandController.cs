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
    Transform _hand;
    Image _handImage;
    HandManager _handManager;
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
            // TracingHand();
        }).AddTo(this);
        _handManager = FindObjectOfType<HandManager>();
        // _canvasRect = _handManager.GetComponent<RectTransform>();
        _hand = _handManager.transform.GetChild(0);
        _handImage = _hand.GetComponent<Image>();
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
            if (_currentname != coll.gameObject.transform.name)
            {
                Debug.Log(coll.gameObject.transform.name);
                _currentname = coll.gameObject.transform.name;
                var touchable = coll.gameObject.GetComponent<ITouchable>();
                if (touchable != null)
                {
                    Debug.Log("クリックできるよ");
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
        _hand.DOScale(Vector3.one * 1.2f, 0.5f);
        HoverOnHandImage();
    }
    void HoverOff()
    {
        _hand.DOScale(Vector3.one, 0.5f);//.OnComplete(() => _isDefaultSize = true);//簡易的に戻す
        HoverOffHandImage();
    }
    protected abstract void HoverOnHandImage();
    protected abstract void HoverOffHandImage();

    void CheckClick()
    {
        if (_isGrabbed)
        {
            if (Input.GetMouseButtonUp(0))
            {
                ClickOff();
            }
        }
        if (!IsClickable) return;
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
                    ClickOn();
                }
                else
                {
                    return;
                }
            }
        }
    }
    void ClickOn()
    {
        _isGrabbed = true;
        ClickOnHandImage();
    }
    void ClickOff()
    {
        _isGrabbed = false;
        ClickOffHandImage();
    }
    protected abstract void ClickOnHandImage();
    protected abstract void ClickOffHandImage();
    protected void ChangeHandImage(Sprite targetSprite)
    {
        _handImage.sprite = targetSprite;
    }
}
