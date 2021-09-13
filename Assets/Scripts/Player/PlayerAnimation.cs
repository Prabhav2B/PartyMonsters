using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject _idleDisplayObject = null;

    private static readonly int _jumpParameterId = Animator.StringToHash("Jump");
    private static readonly int _runningParameterId = Animator.StringToHash("Running");

    private Animator _animator = null;

    private void Awake() => _animator = GetComponentInChildren<Animator>();

    public void Jump() => _animator.SetTrigger(_jumpParameterId);

    public void SetRunning(bool running) => _animator.SetBool(_runningParameterId, running);

    public void SetIdle(bool idle) => _idleDisplayObject.SetActive(idle);

    private void Update()
    {
        if (_animator != null)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Base Layer.Player_idle"))
            {
                _idleDisplayObject.SetActive(true);
            }
            else
            {
                _idleDisplayObject.SetActive(false);
            }
        }
    }
}
