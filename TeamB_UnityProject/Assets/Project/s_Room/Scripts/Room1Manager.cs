using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Room1Manager : RoomPhaseInitializer
{
    [SerializeField]
    AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    // [SerializeField]
    // AudioClip mainSound = default;
    protected override GamePhase SetPhase()
    {
        return GamePhase.Room1;
    }
    protected override async UniTask FirstEvent()
    {
        //今んとこ特に何もなし。
        await UniTask.Yield();
        return;
    }
    protected override async UniTask EndEvent()
    {
        //今んとこ特に何もなし。
        await UniTask.Yield();
        return;
    }
}