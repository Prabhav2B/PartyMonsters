using UnityEngine;
using UnityEngine.Events;
using TMPro;

[
    DisallowMultipleComponent,
    RequireComponent(typeof(Canvas))
]
public class DialogueBox : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textComponent = null;
    [SerializeField]
    private DialogueData _dialogueData = null;
    [SerializeField]
    private UnityEvent _onAdvanceDialogue = new UnityEvent();
    [SerializeField]
    private UnityEvent _onStartDialogue = new UnityEvent();
    [SerializeField]
    private UnityEvent _onEndDialogue = new UnityEvent();

    private int _currentLine = -1;
    private Canvas _canvas = null;

    private void Awake() => _canvas = GetComponent<Canvas>();

    public void StartOrAdvanceDialogue()
    {
        if (_dialogueData != null)
        {
            if (_currentLine < _dialogueData.dialogue.Length - 1)
            {
                if (_currentLine <= -1)
                {
                    _canvas.enabled = true;
                    _onStartDialogue.Invoke();
                    _currentLine = -1;
                    PlayerManager.ActiveInteractableLocked = true;
                }
                else
                {
                    _onAdvanceDialogue.Invoke();
                }
                _textComponent.text = _dialogueData.dialogue[++_currentLine];
            }
            else if (_currentLine > -1)
            {
                EndDialogue();
            }
        }
    }

    public void EndDialogue()
    {
        _canvas.enabled = false;
        _currentLine = -1;
        _textComponent.text = string.Empty;
        _onEndDialogue.Invoke();
        PlayerManager.ActiveInteractableLocked = false;
    }
}
