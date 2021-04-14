using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cinemachine;

public class Room3Manager : RoomPhaseInitializer
{
    [SerializeField]
    AudioSource audioSource = default, radio = default, radioNoise = default, ambient = default;
    // [SerializeField]
    // AudioClip mainSound = default;
    [SerializeField]
    GameObject dogSilhouette = default;
    protected override GamePhase SetPhase()
    {
        return GamePhase.Room3;
    }
    protected override async UniTask FirstEvent()
    {
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        var monologueText = FindObjectOfType<SubtitleCanvas>().monologueText;
        await ShowTextEvent(firstTexts, monologueText);
        // var duration = 3f;
        // await TextAnim.FadeOutText(monologueText, duration, cts.Token);
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
    }
    protected override async UniTask EndEvent()
    {
        //VirtualCameraBlendのため、1フレ待つ必要がある。
        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: cts.Token);
        Debug.Log("end");
        FindObjectOfType<RoomHandController>().SwitchClickable(false);
        //犬が現れる、犬の声がする、ズームインする、クリックを待つ
        if (dogSilhouette == default) Debug.LogWarning("dogSilhouette is not assigned");
        dogSilhouette.SetActive(true);
        var dogVoiceSource = dogSilhouette.GetComponent<AudioSource>();
        if (dogVoiceSource == null) Debug.LogWarning("dogVoiceSource is not assigned");
        dogVoiceSource.Play();
        // 子オブジェクトの中にVirtualCameraをおく。
        var targetVirtualCamera = dogSilhouette.GetComponentsInChildren<CinemachineVirtualCamera>(true)[0].transform;
        if (targetVirtualCamera == null) Debug.LogWarning("VirtualCameraが見つかりません。");
        await FindObjectOfType<PlayerCamController>().ChangeCamera(targetVirtualCamera);
        var monologueText = FindObjectOfType<SubtitleCanvas>().monologueText;
        await ShowTextEvent(endTexts, monologueText);
        var duration = 1.4f;
        TextAnim.FadeOutText(monologueText, duration, cts.Token).Forget();
        FadeOutSound(radio, 1f).Forget();
        FadeOutSound(radioNoise, 1f).Forget();
        FadeOutSound(ambient, 1f).Forget();
        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.Dog3);
        await UniTask.Yield();
    }
}