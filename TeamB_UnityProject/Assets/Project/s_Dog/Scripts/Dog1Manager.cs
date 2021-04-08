using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class Dog1Manager : DogPhaseInitializer
{
    // [SerializeField]
    // AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    [SerializeField]
    float[] eventFlags;
    int _currentState = 0;
    [SerializeField]
    string[] dialogues_ja, dialogues_en;
    string[] _dialogues;
    //UniTaskで反応検知して完結するイベントを作る。
    protected override void StrokeEvent(CancellationToken token)
    {
        if (CheckState() > _currentState)
        {
            _currentState = CheckState();
            Debug.LogWarning("Fire");
        }
    }
    int CheckState()
    {
        float s = strokeSum;
        for (int i = 0; i < eventFlags.Length; i++)
        {
            s -= eventFlags[i];
            if (s < 0)
            {
                return i;
            }
        }
        return eventFlags.Length + 1;
    }
    protected override void DogInit()
    {
        _dialogues = FindObjectOfType<CommonManager>().PlayLang == Lang.ja ? dialogues_ja : dialogues_en;
        //BGM鳴らすなどをここで入れる。
    }


    void LoadNextScene()
    {
        // FadeOutSound(radio, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Night1);
    }
}