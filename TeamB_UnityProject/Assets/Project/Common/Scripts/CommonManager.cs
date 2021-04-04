using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

//もし変更する場合は調整が必要です。
public enum GamePhase{
    Opening,
    News0,
    Room0,
    News1,//TVニュースのアニメーション
    Room1,//コマンド選択可能
    Dog1,//犬シーンの中でどのくらいの段階か選択可能
    Dog1_2,
    News2,
    Room2,
    Dog2,
    Dog2_2,
    News3,
    Room3,
    Dog3,
    Dog3_2,
    News4,
    Room4,
    Dog4,
    Dog4_2,
    Ending
}
//これはパブリックじゃないよ。実際のシーンの名前と同じにする必要があるし、もし変更する場合は色々調整する必要があるよ。
enum GameScene{
    Opening,
    News0,
    Room0,
    News1,
    Room1,
    Dog1,
    News2,
    Room2,
    Dog2,
    News3,
    Room3,
    Dog3,
    News4,
    Room4,
    Dog4,
    Ending
}
public enum Lang
{
    ja,en
}

//Singleton object. Check if any instance already exist in Awake and if yes, destroy itself automaticcaly.
public class CommonManager : SingletonMonoBehaviour<CommonManager> {
    //現在のPhase
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Room1;
    //スタート時のPhase
    [SerializeField]
    GamePhase initialPhase = default;
    //初期パラメーター
    public Lang PlayLang { get; private set; } = Lang.ja;
    public void ChangeLang(Lang lang){ PlayLang = lang; }
    public bool IsDebug { get; private set; } = true;
    public void SwitchIsDebug(bool isDebug) { IsDebug = isDebug; }


    private void Start() {
        Debug.Log("CommonManager起動");
        DontDestroyOnLoad(this.gameObject);
        if(IsDebug){
        //シーン上にマネジメントキャンバスなければ、生成する。一番手前
        var managerUICanvas = FindObjectOfType<ManagerUICanvas>();
        if(managerUICanvas == null){
            var canvas = Instantiate((GameObject)Resources.Load("ManagerUICanvas"));
            //ManagerUICanvasはDon't Destroy
            DontDestroyOnLoad(canvas);
            Debug.Log("Instantiate ManagerUICanvas (Don't destroy)");
        }
        }
        var currentSceneName = SceneManager.GetActiveScene().name;
        CurrentPhase = GetStartPhase(currentSceneName);
        // Debug.Log("InitialPhaseを読み込みます。CommonManagerのインスペクターから設定可能。");
        Debug.Log("CurrentPhase="+CurrentPhase+"InitialPhase="+initialPhase);
        LoadPhase(initialPhase);
    }
    GamePhase GetStartPhase(string sceneName){
        var array = new string[Enum.GetValues(typeof(GameScene)).Length];
        for (int i = 0;i<array.Length;i++){
            var value = (GameScene)Enum.ToObject(typeof(GameScene), i);
            array[i] = value.ToString();
        }
        GamePhase[] startPhaseList = {
            GamePhase.Opening, GamePhase.News0, GamePhase.Room0, GamePhase.News1, GamePhase.Room1, GamePhase.Dog1,
            GamePhase.News2, GamePhase.Dog2, GamePhase.News3,
            GamePhase.Dog3, GamePhase.News4, GamePhase.Dog4, GamePhase.Ending
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
            FadeManager.FadeOut();
            await UniTask.Delay(1500);//ここは今手動になってる。
            await SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Single);
        }
        CurrentPhase = targetPhase;
        Debug.Log("CurrentPhase is " + CurrentPhase);

        //シーン上のPhaseInitializerを検索し、phaseを初期化する。
        FindObjectOfType<PhaseInitializer>().InitializePhase(CurrentPhase);
        //PhaseInitializerにはGamePhase型で分岐し、そのフェイズまで進める機能つける。
    }
    GameScene GetSceneFromPhase(GamePhase phase){
        int value = 0;
        switch(phase){
            case GamePhase.Opening:
                value = 0;
                break;
            case GamePhase.News0:
                value = 1;
                break;
            case GamePhase.Room0:
                value = 2;
                break;
            case GamePhase.News1:
                value = 3;
                break;
            case GamePhase.Room1:
                value = 4;
                break;
            case GamePhase.Dog1:
            case GamePhase.Dog1_2:
                value = 5;
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