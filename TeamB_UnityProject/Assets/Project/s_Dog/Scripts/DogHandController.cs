using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

public class DogHandController : HandController
{
    DogPhaseInitializer _dogPhaseInitializer;

    protected override void SetUpHand()
    {
        var dog = GameObject.Find("dog");
        if (dog == null) Debug.LogError("dog is not in the scene.");
        if (defaultHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (hoverHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        if (clickHandSprite == default) Debug.LogWarning("HandSpriteがアタッチされていません。");
        ChangeHandImage(defaultHandSprite);
        _dogPhaseInitializer = FindObjectOfType<DogPhaseInitializer>();
        if (_dogPhaseInitializer == null) Debug.LogError("DogPhaseInitializer is not fount");
    }
    protected override void Holding(string objName)
    {
        if (objName == "dog")
        {
            // Debug.Log("Stroking");
            _dogPhaseInitializer.StrokingDog();
        }
    }
}
