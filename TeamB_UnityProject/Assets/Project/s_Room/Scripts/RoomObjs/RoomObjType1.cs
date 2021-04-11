using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;
using Cinemachine;

public class RoomObjType1 : RoomObj
{
    protected override async UniTask Next()
    {
        //必要に応じて、ここの分岐の条件は変更し、他の処理を挟めるようにもする。
        if (currentEvent >= eventParams.Count)
        {
            await EndDialogue();
            return;
        }
        //eventParams[currentEvent]の値によって分岐するイベント設定。
        await NextText();
    }
}