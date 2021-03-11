using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandObjectManager : MonoBehaviour
{
    float _z;
    private void Start()
    {
        _z = gameObject.transform.position.z;
    }
    // 位置座標
    private Vector3 position;
    // スクリーン座標をワールド座標に変換した位置座標
    private Vector3 screenToWorldPointPosition;

    // Update is called once per frame
    void Update()
    {
        // Vector3でマウス位置座標を取得する
        position = Input.mousePosition;
        position.x -= Screen.width;
        position.y -= Screen.height;
        position.z = _z;

        // マウス位置座標をスクリーン座標からワールド座標に変換する
        screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(-position);
        Debug.Log(screenToWorldPointPosition);
        screenToWorldPointPosition.z = -_z;
        // ワールド座標に変換されたマウス座標を代入
        gameObject.transform.position = -screenToWorldPointPosition;
    }
}