using UnityEngine;

public class AnimationBool : MonoBehaviour
{
    [SerializeField]
    private string _boolName = string.Empty;

    private int _boolId = default(int);

    private Animator[] _animators = null;

    private void Awake()
    {
        _boolId = Animator.StringToHash(_boolName);
        _animators = GetComponentsInChildren<Animator>();
    }

    public void SetBool(bool value)
    {
        foreach (var animator in _animators)
        {
            animator.SetBool(_boolId, value);
        }
    }
}
