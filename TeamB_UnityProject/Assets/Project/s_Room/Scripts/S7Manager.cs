using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class S7Manager : RoomPhaseInitializer
{
    //flagを用意し、phase移行できるようにする。S1では、ニュース流れるのと、部屋のシーンのphaseがある。
    [NonEditable]
    public int flag;
    int flagAmount;
    public override void InitializePhase(GamePhase targetphase){
        flag = 0;
        flagAmount = FindObjectsOfType<RoomObj>().Length;
        Debug.Log("InitializePhase");
        if(targetphase == GamePhase.s1_Room_News){
            //最初なので特に何も処理はない。
            return;
        }else if(targetphase == GamePhase.s1_Room_Main){
            //Mainまで進む処理をする。
        }else{
            Debug.LogError("phase移行がうまくできていません。Error");
        }
    }
    public override void CheckFlag(){
        // flagみて、全部行ってたら、s1_Mainに進む
        //のちのちとしては、全部じゃなくてもいいかも。
        flag++;
        if(flag >= flagAmount){
            FindObjectOfType<CommonManager>().LoadPhase(GamePhase.s2_Dog_1);
        }
    }
}
