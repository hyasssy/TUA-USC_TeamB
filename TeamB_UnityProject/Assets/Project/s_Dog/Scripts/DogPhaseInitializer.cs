using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class DogPhaseInitializer : PhaseInitializer
{
    protected float strokeSum = 0;
    protected HandController handController;
    //これはHandControllerのIsClickableとやや役割が被っている。
    protected bool stopStroke = false;
    public void StrokingDog()
    {
        if (stopStroke) return;
        var value = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
        if (value != 0)
        {
            // strokeSum += value;
            strokeSum += Time.deltaTime;
            Debug.Log("Stroke dog " + value + ", strokeSum = " + strokeSum);
            StrokeEvent();
            ReactStroke();
        }
    }
    protected abstract void StrokeEvent();
    //撫でた時のリアクション
    protected abstract void ReactStroke();

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (targetphase == GamePhase.Dog1)
        {
            DogMain();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    void DogMain()
    {
        FadeManager.FadeIn();
        //クリックできるかを初期化する。
        handController = FindObjectOfType<HandController>();
        handController.SwitchClickable(true);
        stopStroke = false;
        //各シーンごとの初期化
        DogInit();
    }
    abstract protected void DogInit();

}
