using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1Manager : PhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    public override void InitializePhase(GamePhase currentphase){
        Debug.Log("InitializePhase");
        if(currentphase == GamePhase.s1_Room_News){
            //最初なので特に何も処理はない。
            return;
        }else if(currentphase == GamePhase.s1_Room_Main){
            //Mainまで進む処理をする。
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
}
