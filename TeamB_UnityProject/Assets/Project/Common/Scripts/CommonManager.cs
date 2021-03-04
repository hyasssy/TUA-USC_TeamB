using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;


public enum GamePhase{
    s0_Opening,
    s1_Room_Start,//TVニュースのアニメーション
    s1_Room_Main,//コマンド選択可能
    s2_Dog_1,//犬シーンの中でどのくらいの段階か選択可能
    s2_Dog_2,
    s3_Room_Start,
    s3_Room_Main,
    s4_Dog_1,
    s4_Dog_2,
    s5_Room_Start,
    s5_Room_Main,
    s6_Dog_1,
    s6_Dog_2,
    s7_Room_Start,
    s7_Room_Main,
    s8_Dog_1,
    s8_Dog_2,
    s9_Ending
}
public enum GameScene{
    s0_Opening,
    s1_Room,
    s2_Dog,
    s3_Room,
    s4_Dog,
    s5_Room,
    s6_Dog,
    s7_Room,
    s8_Dog,
    s9_Ending
}

//Singleton object. Check if any instance already exist in Awake and if yes, destroy itself automaticcaly.
public class CommonManager : SingletonMonoBehaviour<CommonManager> {
    //現在の
    [field: SerializeField, RenameField(nameof(CurrentPhase))]
    public GamePhase CurrentPhase { get; private set; } = GamePhase.s1_Room_Main;
    [SerializeField]
    GamePhase initialPhase = default;

    private void Start() {
        Debug.Log("CommonManager起動");
        DontDestroyOnLoad(this.gameObject);
        //シーン上にマネジメントキャンバスなければ、生成する。一番手前
        var managerUICanvas = FindObjectOfType<ManagerUICanvas>();
        if(managerUICanvas == null){
            var canvas = Instantiate((GameObject)Resources.Load("ManagerUICanvas"));
            //ManagerUICanvasはDon't Destroy
            DontDestroyOnLoad(canvas);
            Debug.Log("Instantiate ManagerUICanvas (Don't destroy)");
        }
        var currentSceneName = SceneManager.GetActiveScene().name;
        CurrentPhase = GetStartPhase(currentSceneName);
        if(initialPhase == default) initialPhase = CurrentPhase;
        LoadState(initialPhase);
    }
    GamePhase GetStartPhase(string sceneName){
        var array = new string[Enum.GetValues(typeof(GameScene)).Length];
        for (int i = 0;i<array.Length;i++){
            var value = (GameScene)Enum.ToObject(typeof(GameScene), i);
            array[i] = value.ToString();
        }
        GamePhase[] startPhaseList = {
            GamePhase.s0_Opening, GamePhase.s1_Room_Start, GamePhase.s2_Dog_1,
            GamePhase.s3_Room_Start, GamePhase.s4_Dog_1, GamePhase.s5_Room_Start,
            GamePhase.s6_Dog_1, GamePhase.s7_Room_Start, GamePhase.s8_Dog_1, GamePhase.s9_Ending
        };
        GamePhase result = default;
        for (int i = 0; i < array.Length; i++) {
            if(sceneName == array[i]){
                result = startPhaseList[i];
            }
        }
        return result;
    }


    //フェイズ管理プログラム作成

    public void LoadState(GamePhase targetPhase){
        Debug.Log("Next phase name is " + targetPhase);
        LoadStateTask(targetPhase).Forget();

    }
    //UIからDropdownを選んだときの処理。
    public void LoadState(int targetStateNum){
        var targetPhase = (GamePhase)Enum.ToObject(typeof(GamePhase), targetStateNum);
        Debug.Log("Selected number is " + targetStateNum + ", Phase name is " + targetPhase);
        LoadStateTask(targetPhase).Forget();
    }
    async UniTask LoadStateTask(GamePhase targetPhase){
        var currentScene = GetSceneFromPhase(CurrentPhase);
        var targetScene = GetSceneFromPhase(targetPhase);
        if(currentScene != targetScene){
            Debug.Log("Load the different scene");
            await SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Single);
        }
        CurrentPhase = targetPhase;
        Debug.Log("CurrentPhase is " + CurrentPhase);
        //シーン上のStateManagerインターフェースを検索し、ShiftState(CurrentPhase)を実装する。
        //ShiftState(CurrentPhase);
        //StateManagerにはGamePhase型で分岐し、そのステートまで進める機能つける。
        //もしインターフェースが見つからなかったら
        // Debug.LogAssertion("StateManagerがシーン上に見つかりません！");
    }
    GameScene GetSceneFromPhase(GamePhase phase){
        int value = 0;
        switch(phase){
            case GamePhase.s0_Opening:
                value = 0;
                break;
            case GamePhase.s1_Room_Main:
            case GamePhase.s1_Room_Start:
                value = 1;
                break;
            case GamePhase.s2_Dog_1:
            case GamePhase.s2_Dog_2:
                value = 2;
                break;
            default:
                Debug.Log("まだ適切な値が実装されてないよ");
            break;
            //以下とりあえず省略。
        }
        var result = (GameScene)Enum.ToObject(typeof(GameScene), value);
        return result;
    }
}