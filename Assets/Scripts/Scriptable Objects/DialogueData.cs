using System;
using UnityEngine;

[Serializable]
public struct DialogueItem
{
    [TextArea(3, 3)]
    public string text;
    [Min(0f)]
    public float delay;
    public QuestData quest;
    [TextArea(3, 3)]
    public string questFailText;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "Data/Dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField]
    private DialogueItem[] _sequence = new DialogueItem[0];
    [SerializeField]
    private DialogueItem[] _afterQuestSequence = new DialogueItem[0];

    [NonSerialized]
    public bool questLineFinished = false;

    public DialogueItem[] sequence => (questLineFinished) ? _afterQuestSequence : _sequence;
}
