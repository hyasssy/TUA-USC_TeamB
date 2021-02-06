using UnityEngine;

public class DisplayDebugLogSwitcher : MonoBehaviour
{
    [SerializeField, Tooltip("このキーを3秒長押しすると表示非表示切り替える")]
    private KeyCode displayLogSwitchKey = KeyCode.C;//default : C of Console
    GameObject _debugLogPanel;
    [SerializeField]
    float pushDuration = 2.5f;
    float timeCount = 0f;
    private void Start() {
        _debugLogPanel = transform.GetChild(0).gameObject;
    }
    private void Update() {
        if(Input.GetKey(displayLogSwitchKey)){
            timeCount += Time.deltaTime;
            if (timeCount >= pushDuration)
            {
                _debugLogPanel.SetActive(!_debugLogPanel.activeSelf);
                if(_debugLogPanel.activeSelf) Debug.Log("Activate Debug Log View");
                timeCount = 0;
            }
        }
        if(Input.GetKeyUp(displayLogSwitchKey)){
            timeCount = 0;
        }
    }
}
