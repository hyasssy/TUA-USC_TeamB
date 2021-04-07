using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HandManager : MonoBehaviour
{
    Vector3 _mousePos;
    RectTransform _canvasRect;
    Transform _hand;
    public Vector2 targetCursorPos = new Vector2(30f, -80f);
    public float handMoveSpeed = 1f;
    private void Start()
    {
        _canvasRect = GetComponent<RectTransform>();
        _hand = transform.GetChild(0);

        this.UpdateAsObservable()
        .Subscribe(_ =>
        {
            TracingHand();
        });
    }
    void TracingHand()
    {
        _mousePos = Input.mousePosition;
        _mousePos.x = Mathf.Clamp(_mousePos.x, 0, Screen.width);
        _mousePos.y = Mathf.Clamp(_mousePos.y, 0, Screen.height);
        Debug.Log(_mousePos);
        var magnification = _canvasRect.sizeDelta.x / Screen.width;
        _mousePos.x = _mousePos.x * magnification - _canvasRect.sizeDelta.x / 2 + targetCursorPos.x;
        _mousePos.y = _mousePos.y * magnification - _canvasRect.sizeDelta.y / 2 + targetCursorPos.y;
        _mousePos.z = transform.localPosition.z;

        _hand.localPosition = Vector3.Lerp(_hand.localPosition, _mousePos, Time.deltaTime * handMoveSpeed);
    }
}
