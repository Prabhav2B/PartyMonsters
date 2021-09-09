using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Data/Dialogue")]
public class DialogueData : ScriptableObject
{
    [
        SerializeField,
        TextArea(3, 3)
    ]
    public string[] dialogue = new string[0];
}
