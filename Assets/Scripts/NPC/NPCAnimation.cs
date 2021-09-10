using UnityEngine;

[DisallowMultipleComponent]
public class NPCAnimation : MonoBehaviour
{
    private static readonly int _speakParameterId = Animator.StringToHash("Speak");

    private Animator _animator = null;

    private void Awake() => _animator = GetComponentInChildren<Animator>();

    public void Speak() => _animator.SetTrigger(_speakParameterId);
}
