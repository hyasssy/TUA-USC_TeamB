using UnityEngine;
using System.Collections.Generic;

public class PMTest : MonoBehaviour
{
	// FSMがあるGameObject
	public GameObject obj;

	// 呼び出したいFSM名
	public string FSMreferenceName;

	// 呼び出したいイベント名
	public string eventText;

	private PlayMakerFSM[] FSMs;

	void Start ()
	{
		// 初期時にobjにあるFSMを取得しておく
		FSMs = obj.GetComponents<PlayMakerFSM>();
	}

	// イベントを送信する関数
	void SendEvent ()
	{
		foreach (PlayMakerFSM fsm in FSMs)
		{
			if (fsm.FsmName == FSMreferenceName)
			{
				fsm.Fsm.Event(eventText);
			}
		}
	}
}
