﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class RoomPhaseInitializer : PhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    int flagAmount;

    protected override void InitializePhase(GamePhase targetphase)
    {
        if (targetphase == SetPhase())
        {
            RoomMain();
        }
        else
        {
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    abstract protected GamePhase SetPhase();
    void RoomMain()
    {
        FadeManager.FadeIn();
        FindObjectOfType<PlayerCamController>().SetDefaultCamera();
        FindObjectOfType<SubtitleCanvas>().SetUpTexts();
        //選択可能をリセットする
        flag = 0;
        var roomObjs = FindObjectsOfType<RoomObj>();

        roomObjs.ToList().ForEach(r =>
        {
            //重要なものだけカウントする。
            if (r.IsImportant) flagAmount++;
            r.SetUpRoomItem();
        });
        //クリックできるかを初期化する。
        FindObjectOfType<RoomHandController>().SwitchClickable(true);
        //音鳴らす
        // PlaySound();
        //テキスト出すなど、シーン最初のイベントを設定
        FirstEvent();
    }
    abstract protected UniTask FirstEvent();
    public void CheckFlag()
    {
        // flagみて、全部行ってたら、dogシーンに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if (flag >= flagAmount)
        {
            EndEvent().Forget();
        }
    }
    abstract protected UniTask EndEvent();
}
