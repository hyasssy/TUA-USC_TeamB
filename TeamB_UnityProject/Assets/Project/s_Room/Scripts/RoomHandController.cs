﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

public class RoomHandController : HandController
{
    protected override void SetUpHand()
    {
        if (defaultHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (hoverHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (clickHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        ChangeHandImage(defaultHandSprite);
    }
    protected override void Holding(string objName)
    {
        //部屋では特になし。
    }
}