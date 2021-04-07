using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogHandController : HandController
{
    [SerializeField]
    Sprite defaultHandSprite = default, hoverHandSprite = default, clickHandSprite = default;

    protected override void SetUpHand()
    {
        if (defaultHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (hoverHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (clickHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        ChangeHandImage(defaultHandSprite);
    }
    protected override void HoverOnHandImage()
    {
        ChangeHandImage(hoverHandSprite);
    }
    protected override void HoverOffHandImage()
    {
        ChangeHandImage(defaultHandSprite);
    }
    protected override void ClickOnHandImage()
    {
        ChangeHandImage(clickHandSprite);
    }
    protected override void Clicking(string objName)
    {
        if (objName == "dog")
        {
            Debug.Log("Scraching");
        }
    }
    protected override void ClickOffHandImage()
    {
        ChangeHandImage(defaultHandSprite);
    }
}
