using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

//もし変更する場合は調整が必要です。
public enum GamePhase{
    s0_Opening,
    s1_Room_News,//TVニュースのアニメーション
    s1_Room_Main,//コマンド選択可能
    s2_Dog_1,//犬シーンの中でどのくらいの段階か選択可能
    s2_Dog_2,
    s3_Room_News,
    s3_Room_Main,
    s4_Dog_1,
    s4_Dog_2,
    s5_Room_News,
    s5_Room_Main,
    s6_Dog_1,
    s6_Dog_2,
    s7_Room_News,
    s7_Room_Main,
    s8_Dog_1,
    s8_Dog_2,
    s9_Ending
}
//これはパブリックじゃないよ。実際のシーンの名前と同じにする必要があるし、もし変更する場合は色々調整する必要があるよ。
enum GameScene{
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
    //現在のPhase
    public GamePhase CurrentPhase { get; private set; } = GamePhase.s1_Room_Main;
    //スタート時の
    [SerializeField]
    GamePhase initialPhase = default;
    [SerializeField]
    bool isFade = true;
    public bool IsClickable {get; private set; } = true;
    public void SwitchClickable(bool value){
        IsClickable = value;
    }

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
        if(isFade) FadeManager.FadeIn();
        var currentSceneName = SceneManager.GetActiveScene().name;
        CurrentPhase = GetStartPhase(currentSceneName);
        Debug.Log("InitialPhaseを読み込みます。CommonManagerのインスペクターから設定可能。");
        LoadPhase(initialPhase);
    }
    GamePhase GetStartPhase(string sceneName){
        var array = new string[Enum.GetValues(typeof(GameScene)).Length];
        for (int i = 0;i<array.Length;i++){
            var value = (GameScene)Enum.ToObject(typeof(GameScene), i);
            array[i] = value.ToString();
        }
        GamePhase[] startPhaseList = {
            GamePhase.s0_Opening, GamePhase.s1_Room_News, GamePhase.s2_Dog_1,
            GamePhase.s3_Room_News, GamePhase.s4_Dog_1, GamePhase.s5_Room_News,
            GamePhase.s6_Dog_1, GamePhase.s7_Room_News, GamePhase.s8_Dog_1, GamePhase.s9_Ending
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
    public void LoadPhase(GamePhase targetPhase){
        Debug.Log("Next phase name is " + targetPhase);
        LoadPhaseTask(targetPhase).Forget();
    }
    //UIからDropdownを選んだときの処理。
    public void LoadPhase(int targetPhaseNum){
        var targetPhase = (GamePhase)Enum.ToObject(typeof(GamePhase), targetPhaseNum);
        Debug.Log("Selected number is " + targetPhaseNum + ", Next phase name is " + targetPhase);
        LoadPhaseTask(targetPhase).Forget();
    }
    async UniTask LoadPhaseTask(GamePhase targetPhase){
        var currentScene = GetSceneFromPhase(CurrentPhase);
        var targetScene = GetSceneFromPhase(targetPhase);
        if(currentScene != targetScene){
            Debug.Log("Load the different scene");
            if(isFade){
                FadeManager.FadeOut();
                await UniTask.Delay(1500);//ここは今手動になってる。
            }
            await SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Single);
            if(isFade) FadeManager.FadeIn();
        }
        CurrentPhase = targetPhase;
        Debug.Log("CurrentPhase is " + CurrentPhase);
        //クリックできるかを初期化する。
        IsClickable = true;
        //シーン上のPhaseInitializerを検索し、phaseを初期化する。
        FindObjectOfType<PhaseInitializer>().InitializePhase(CurrentPhase);
        //PhaseInitializerにはGamePhase型で分岐し、そのフェイズまで進める機能つける。
    }
    GameScene GetSceneFromPhase(GamePhase phase){
        int value = 0;
        switch(phase){
            case GamePhase.s0_Opening:
                value = 0;
                break;
            case GamePhase.s1_Room_Main:
            case GamePhase.s1_Room_News:
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