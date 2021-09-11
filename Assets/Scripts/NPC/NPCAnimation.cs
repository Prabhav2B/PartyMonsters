using UnityEngine;

[DisallowMultipleComponent]
public class NPCAnimation : MonoBehaviour
{
    private static readonly int _speakParameterId = Animator.StringToHash("Speak");

    private Animator[] _animators = null;

    private void Awake() => _animators = GetComponentsInChildren<Animator>();

    public void Speak()
    {
        foreach (var animator in _animators)
        {
            animator.SetTrigger(_speakParameterId);
        }
    }
}
