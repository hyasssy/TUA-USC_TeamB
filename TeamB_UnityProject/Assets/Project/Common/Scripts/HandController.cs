using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField]
    Texture2D cursorImage = default;
    [SerializeField]
    int handSize = 150;

    //これ安定しないから、ずっとシーンにオブジェおいて、マウスに追従させた方がいいかも。
    private void Awake() {
        Cursor.visible = false;
        TextureScale.Bilinear(cursorImage, handSize, handSize);
        var pointfingerPos = new Vector2(handSize / 5, handSize / 7);
        Cursor.SetCursor(cursorImage, pointfingerPos, CursorMode.Auto);
    }
    //hoverしたら画像差し替え
    //クリックしたら画像差し替え
}