using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/SetParam")]
public class SetParam : ScriptableObject
{
    [field: SerializeField, RenameField(nameof(TextTypingSpeed))]
    public float TextTypingSpeed { get; private set; } = 0.05f;
    [field: SerializeField, RenameField(nameof(VoiceTypingSpeed))]
    public float VoiceTypingSpeed { get; private set; } = 0.1f;
}
