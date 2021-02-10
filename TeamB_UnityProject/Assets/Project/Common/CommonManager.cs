using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton object. Check if any instance already exist in Awake and if yes, destroy itself automaticcaly.
public class CommonManager : SingletonMonoBehaviour<CommonManager> {
    //ボタンを長押ししたらシーン遷移ボタンプレファブを生成、表示・非表示
    public void DisplaySceneManager () {
        // FindObjectOfType<> ();
    }
}