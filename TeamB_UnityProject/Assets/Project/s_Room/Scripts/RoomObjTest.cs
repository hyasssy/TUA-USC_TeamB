using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjTest : MonoBehaviour, ITouchable
{
    public void Clicked(){
        Debug.Log("クリックされた");
    }
}
