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



public class OpeningManager : PhaseInitializer
{
    //言語選択
    public bool IsJapanese {get;private set;} = true;
    [SerializeField]
    Toggle toggle_ja = default, toggle_en = default;

    public void StartGame(){

    }

    protected override void InitializePhase(GamePhase targetphase)
    {
        if(toggle_ja == default || toggle_en == default) Debug.LogWarning("toggle_ja or toggle_en is not assigned.");
        Debug.Log (toggle_ja.isOn + ", " + toggle_en.isOn);
        Debug.Log("InitializePhase");
        if (targetphase == GamePhase.Opening)
        {
            // RoomNews().Forget();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }


    async UniTask RoomNews()
    {
        

        FindObjectOfType<CommonManager>().LoadPhase(GamePhase.News0);
        StopSound();
    }
    void StopSound()
    {

    }
}
