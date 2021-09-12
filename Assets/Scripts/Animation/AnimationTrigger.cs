using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField]
    private string _triggerName = string.Empty;

    private int _triggerId = default(int);

    private Animator[] _animators = null;

    private void Awake()
    {
        _triggerId = Animator.StringToHash(_triggerName);
        _animators = GetComponentsInChildren<Animator>();
    }

    public void Trigger()
    {
        foreach (var animator in _animators)
        {
            animator.SetTrigger(_triggerId);
        }
    }
}
