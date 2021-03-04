using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton object. Check if any instance already exist in Awake and if yes, destroy itself automaticcaly.
public class CommonManager : SingletonMonoBehaviour<CommonManager> {
    //現在の
    public GamePhase CurrentPhase { get; private set; } = GamePhase.S1_Opening;
    public void PhaseShift(GamePhase gp){
        CurrentPhase = gp;
    }

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        //シーン上にマネジメントキャンバスなければ、生成する。一番手前
        // var stateManagePanel = FindObjectOfType<>
        //処理
    }
    public void DisplaySceneManager () {
        // FindObjectOfType<> ();
    }

    //フェイズ管理プログラム作成
}
public enum GamePhase{
    S1_Opening,
    S2_Room,
    S3_Dog,

    //今省略

    S9_Dog,
    S10_Ending
}