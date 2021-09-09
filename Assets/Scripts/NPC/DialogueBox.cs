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
    private bool _questFailed = false;
    private bool _questAccomplished = false;
    private Canvas _canvas = null;

    private void Awake() => _canvas = GetComponent<Canvas>();

    public void StartOrAdvanceDialogue()
    {
        if (_dialogueData != null)
        {
            if (_currentLine < _dialogueData.sequence.Length - 1 && !_questFailed)
            {
                if (_currentLine <= -1)
                {
                    StartDialogue();
                }
                else
                {
                    var quest = _dialogueData.sequence[_currentLine].quest;
                    if (quest != null)
                    {
                        _questAccomplished = quest.TryAccomplishQuest();
                        _questFailed = !_questAccomplished;
                    }
                    _onAdvanceDialogue.Invoke();
                }
                _textComponent.text = (_questFailed)? _dialogueData.sequence[_currentLine].questFailText : _dialogueData.sequence[++_currentLine].text;
            }
            else if (_currentLine > -1)
            {
                EndDialogue();
                _dialogueData.questLineFinished = _questAccomplished;
            }
        }
    }

    public void EndDialogue()
    {
        _canvas.enabled = false;
        _currentLine = -1;
        _questFailed = false;
        _textComponent.text = string.Empty;
        _onEndDialogue.Invoke();
        PlayerManager.ActiveInteractableLocked = false;
    }

    private void StartDialogue()
    {
        _canvas.enabled = true;
        _onStartDialogue.Invoke();
        _currentLine = -1;
        PlayerManager.ActiveInteractableLocked = true;
    }
}
