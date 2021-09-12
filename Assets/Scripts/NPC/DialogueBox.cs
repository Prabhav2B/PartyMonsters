using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

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
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeInDuration = 0f;
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeOutDuration = 0f;
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
    private CanvasGroup _canvasGroup = null;

    private bool _lockDialogue = false;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartOrAdvanceDialogue()
    {
        if (!_lockDialogue)
        {
            StartCoroutine(DialogueCoroutine());
        }
    }

    public IEnumerator DialogueCoroutine()
    {
        if (_dialogueData != null)
        {
            _lockDialogue = true;
            if (_currentLine < _dialogueData.sequence.Length - 1 && !_questFailed)
            {
                bool started = false;
                if (_currentLine <= -1)
                {
                    StartDialogue();
                    started = true;
                }
                else
                {
                    yield return FadeFromTo(1f, 0f, _fadeOutDuration);

                    var quest = _dialogueData.sequence[_currentLine].quest;
                    if (quest != null)
                    {
                        _questAccomplished = quest.TryAccomplishQuest();
                        _questFailed = !_questAccomplished;
                        if (_questFailed)
                        {
                            _textComponent.text = _dialogueData.sequence[_currentLine].questFailText;
                            _onAdvanceDialogue.Invoke();
                            yield return FadeFromTo(0f, 1f, _fadeInDuration);
                            _lockDialogue = false;
                            yield break;
                        }
                    }
                }
                var nextLine = _dialogueData.sequence[++_currentLine];
                _textComponent.text = nextLine.text;
                if (nextLine.delay > 0f)
                {
                    _canvas.enabled = false;
                    yield return new WaitForSeconds(nextLine.delay);
                    _canvas.enabled = true;
                }
                if (started)
                {
                    _onStartDialogue.Invoke();
                }
                else
                {
                    _onAdvanceDialogue.Invoke();
                }
                yield return FadeFromTo(0f, 1f, _fadeInDuration);
            }
            else if (_currentLine > -1)
            {
                EndDialogue();
            }
            _dialogueData.questLineFinished = _questAccomplished;
            _lockDialogue = false;
        }
        yield return null;
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        _canvasGroup.alpha = from;
        var fade = _canvasGroup.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }

    public void EndDialogue()
    {
        StartCoroutine(EndDialogueCoroutine());
    }

    private IEnumerator EndDialogueCoroutine()
    {
        if (_currentLine > -1)
        {
            yield return FadeFromTo(1f, 0f, _fadeOutDuration);
            _canvas.enabled = false;
            _currentLine = -1;
            _questFailed = false;
            _lockDialogue = false;
            _textComponent.text = string.Empty;
            _onEndDialogue.Invoke();
            PlayerManager.ActiveInteractableLocked = false;
        }
    }

    private void StartDialogue()
    {
        _canvas.enabled = true;
        _currentLine = -1;
        PlayerManager.ActiveInteractableLocked = true;
    }
}
