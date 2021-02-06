using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    //欲しい機能
    //全てのインプット系をここを経由させて用いることで、何がどこで使われているかを参照できるようにする。
    //キーマップの変更などを管理する。
    //このメソッドで全てまたは一部の入力を一時受付禁止できるようにする。
    //GetAxisDownを実装
    //AnyDownを実装
    //クラスをserializableで作って、とかの方がいいか
    private Dictionary<Button, KeyCode[]> padButtons = new Dictionary<Button, KeyCode[]>(){
        {Button.Jump, new KeyCode[]{KeyCode.Space}},
        {Button.Fire1, new KeyCode[]{KeyCode.Return}},
        {Button.Left, new KeyCode[]{KeyCode.A, KeyCode.LeftArrow}},
        {Button.Right, new KeyCode[]{KeyCode.D, KeyCode.RightArrow}},
        {Button.Up, new KeyCode[]{KeyCode.UpArrow, KeyCode.DownArrow}}
        // {Button.Up, KeyCode.W},
        // {Button.Down, KeyCode.S},
    };
    public enum Button
    {
        Jump, Fire1, Left, Right, Up, Down, Debug//など。必要な分設定
    }
    public enum Axis
    {
        R_Horizontal, R_Vertical, L_Horizontal, L_Vertical//必要な分設定
    }

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }


    public bool GetKey(Button button){
        bool value = false;
        for (int i = 0; i < padButtons[button].Length;i++){
            if(Input.GetKey(padButtons[button][i])){
                value = true;
            }
        }
        return value;
    }

}
