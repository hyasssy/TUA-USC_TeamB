using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Dog1Manager : DogPhaseInitializer
{
    // [SerializeField]
    // AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    // [SerializeField]
    // AudioClip mainSound = default;
    // protected override void PlaySound(){
    //     radio.Play();
    //     radioNoise.Play();
    //     ambient.Play();
    // }
    // protected override void LoadNextScene(){
    //     FadeOutSound(radio, 1f).Forget();
    //     FadeOutSound(radioNoise, 1f).Forget();
    //     FadeOutSound(ambient, 1f).Forget();
    //     FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Dog1);
    // }
}