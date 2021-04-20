using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.UI;



public class VoidManager : PhaseInitializer
{
    protected override void InitializePhase(GamePhase targetphase)
    {
        switch(targetphase){
            case GamePhase.Void:
                Void(GamePhase.Room0).Forget();
                break;
            default:
            Debug.LogError("phase移行がうまくできていません。Error");
            break;
        }
    }
    async UniTask Void(GamePhase nextPhase)
    {
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        FindObjectOfType<CommonManager>().LoadPhase(nextPhase);
        return;
    }
}
