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

public class RO_OnlyText : RoomObj
{
    protected override async UniTask Next(CancellationToken token){
        //必要に応じて、ここの分岐の条件は変更し、他の処理を挟めるようにもする。
        if(currentTextNum < dialogues.Count){
            await NextText();
        }else{
            EndDialogue().Forget();
        }
    }
}
