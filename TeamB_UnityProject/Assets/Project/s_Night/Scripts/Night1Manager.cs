using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Night1Manager : NightPhaseInitializer
{
    protected override GamePhase SetPhase()
    {
        return GamePhase.Night1;
    }
    protected override GamePhase SetNextPhase()
    {
        Debug.LogWarning("アルファ版のため、Room3に飛びます。");
        return GamePhase.Room3;
    }
}