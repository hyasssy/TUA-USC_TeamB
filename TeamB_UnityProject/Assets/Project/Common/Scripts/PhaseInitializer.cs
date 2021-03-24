using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//抽象クラスでInitializePhaseを実装。
public abstract class PhaseInitializer : MonoBehaviour
{
    private void Start() {
        var list = FindObjectsOfType<PhaseInitializer>().ToList();
        //たくさんあるとまずい。シーン上に一つの想定
        list.ForEach(obj => Debug.Log("SceneInitializeを持ったオブジェクト:" + obj.gameObject));
    }
    abstract public void InitializePhase(GamePhase currentphase);
}