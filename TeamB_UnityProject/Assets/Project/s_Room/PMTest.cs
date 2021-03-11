using UnityEngine;
using System.Collections.Generic;
using System;

public class PMTest : MonoBehaviour
{

	// 呼び出したいFSM名
	public string FSMreferenceName;

	// 呼び出したいイベント名
	public string eventText;

	private PlayMakerFSM[] FSMs;

	void Start ()
	{
		// 初期時にobjにあるFSMを取得しておく
		FSMs = GetComponents<PlayMakerFSM>();
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
