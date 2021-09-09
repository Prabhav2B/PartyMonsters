using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[
    DisallowMultipleComponent,
    RequireComponent(typeof(Collider2D))
]
public class Interactable : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer = default(LayerMask);
    [
        SerializeField,
        Min(0f)
    ]
    private float _reactivationInterval = 0f;
    [SerializeField]
    private UnityEvent _onInteract = new UnityEvent();
    [SerializeField]
    private UnityEvent _onLeaveArea = new UnityEvent();

    private bool _hideInteractionSprite = false;
    private bool _canBeActivated = true;
    private bool _active = false;
    public bool Active => _active;

    private SpriteRenderer _interactionSprite = null;
    private WaitForSeconds _waitForReactivationInterval = null;
    
    private void Awake()
    {
        _interactionSprite = GetComponentInChildren<SpriteRenderer>();
        _waitForReactivationInterval = new WaitForSeconds(_reactivationInterval);
        Deactivate();
    }

    private void OnEnable()
    {
        _canBeActivated = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_playerLayer & 1 << other.gameObject.layer) != 0)
        {
            other.GetComponent<PlayerManager>().AllowInteraction(this);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if ((_playerLayer & 1 << other.gameObject.layer) != 0)
        {
            other.GetComponent<PlayerManager>().DisallowInteraction(this);
            _onLeaveArea.Invoke();
        }
    }

    public void Interact()
    {
        if (_active)
        {
            _onInteract.Invoke();
            if (_reactivationInterval > Mathf.Epsilon)
            {
                Deactivate();
                StartCoroutine(ReactivationTimer());
            }
        }
    }

    public void Activate()
    {
        if (_canBeActivated)
        {
            if (_interactionSprite != null && !_hideInteractionSprite)
            {
                _interactionSprite.enabled = true;
            }
            _active = true;
        }
    }

    public void Deactivate()
    {
        if (_interactionSprite != null)
        {
            _interactionSprite.enabled = false;
        }
        _active = false;
    }

    public void ShowInteractionSprite()
    {
        _hideInteractionSprite = false;
        if (_interactionSprite != null && _active)
        {
            _interactionSprite.enabled = true;
        }
    }

    public void HideInteractionSprite()
    {
        _hideInteractionSprite = true;
        if (_interactionSprite != null)
        {
            _interactionSprite.enabled = false;
        }
    }

    private IEnumerator ReactivationTimer()
    {
        _canBeActivated = false;
        yield return _waitForReactivationInterval;
        _canBeActivated = true;
    }
}
