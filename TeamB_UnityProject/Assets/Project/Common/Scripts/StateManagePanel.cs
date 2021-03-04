using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagePanel : MonoBehaviour
{
    public void SwitchActive(GameObject go){
        go.SetActive(!go.activeInHierarchy);
    }
}
