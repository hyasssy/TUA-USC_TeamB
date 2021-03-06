using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManagePanel : MonoBehaviour
{
    public void SwitchActive(GameObject go){
        go.SetActive(!go.activeInHierarchy);
    }
}
