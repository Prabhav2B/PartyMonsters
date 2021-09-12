using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class QuestListener : MonoBehaviour
{
    [SerializeField]
    private QuestData _quest = null;
    [SerializeField]
    private UnityEvent _onAccomplished = new UnityEvent();
    [SerializeField]
    private UnityEvent _onFailed = new UnityEvent();

    private void OnEnable()
    {
        _quest.OnAccomplished += QuestAccomplished;
        _quest.OnFailed += QuestFailed;
    }

    private void OnDisable()
    {
        _quest.OnAccomplished -= QuestAccomplished;
        _quest.OnFailed -= QuestFailed;
    }

    private void QuestAccomplished() => _onAccomplished.Invoke();
    private void QuestFailed() => _onFailed.Invoke();
}
