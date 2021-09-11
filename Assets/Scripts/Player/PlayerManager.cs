using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(CharController))]
public class PlayerManager : SingleInstance<PlayerManager>
{
    [SerializeField]
    private ObjectToggler _mapToggler = null;

    private SpriteRenderer _characterSpriteRenderer;
    private ParticleSystem _particleSystem;
    private Transform _spriteRendererTransform;
    private CharController _characterController;
    private Vector3 _lastCollisionPoint;
    private float _lastCollisionNormal;
    private int _movementTweenFlag;
    private PlayerInput _input;
    private PlayerAnimation _animation;
    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;
    private TrainScheduler _trainScheduler;
    private MainMenu _mainMenu;
    private Vector3 _initalPosition;
    private bool _pause;
    private bool _blockMovement;

    private readonly List<Interactable> _interactables = new List<Interactable>();
    private Interactable _activeInteractable = null;

    public static bool ActiveInteractableLocked = false;

    protected Vector2 MoveVector;
    public Vector2 ReceivedInput { get; private set; }

    public Action OnLand;

    public ControlScheme CurrentControlScheme { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInput>();
        _animation = GetComponent<PlayerAnimation>();
        _characterController = GetComponent<CharController>();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
        _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
        _trainScheduler = FindObjectOfType<TrainScheduler>();
        _mainMenu = FindObjectOfType<MainMenu>();

        _characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRendererTransform = _characterSpriteRenderer.transform;

        //_particleSystem = this.GetComponentInChildren<ParticleSystem>();
        UpdateCurrentScheme(_input.currentControlScheme);

        //Deactivate();
    }

    private void Start()
    {
        _trainScheduler.OnTrain = false;
        _initalPosition = transform.position;
        _movementTweenFlag = -1;
    }

    protected void OnEnable()
    {
        InputUser.onChange += OnInputDeviceChange;
        _characterController.OnJump += _animation.Jump;
    }

    protected void OnDisable()
    {
        InputUser.onChange -= OnInputDeviceChange;
        _characterController.OnJump -= _animation.Jump;
    }

    public void ResetScene()
    {
        Deactivate(); //Turn of input from user
        SceneFadeManager.PostFadeOut fadeOutAction = _sceneFadeManager.FadeIn;
        fadeOutAction += Activate;
        _sceneFadeManager.FadeOut(fadeOutAction);
        fadeOutAction = null;
    }

    // public void OnDoubleClickTest(InputAction.CallbackContext context)
    // {
    //     if(context.interaction is MultiTapInteraction && context.performed)
    //         Debug.Log("Double CLick");
    // }
    //
    // public void OnHoldTest(InputAction.CallbackContext context)
    // {
    //     if(context.interaction is HoldInteraction && context.performed)
    //         Debug.Log("Holding");
    // }
    
    private void ResetPlayerPosition()
    {
        transform.position = _initalPosition;
    }

    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            if (user.controlScheme != null) UpdateCurrentScheme(user.controlScheme.Value.name);
        }
    }

    private void UpdateCurrentScheme(string schemeName)
    {
        CurrentControlScheme = schemeName.Equals("Gamepad") ? ControlScheme.Gamepad : ControlScheme.KeyboardAndMouse;
    }
    
    public void OnMainMenu(InputAction.CallbackContext context)
    {
        if(context.performed)
            _mainMenu.TriggerMainMenu();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(_pause || _blockMovement)
            return;
        
        ReceivedInput = context.ReadValue<Vector2>();
        _characterController.Move(ReceivedInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(_pause || _blockMovement)
            return;
        
        if (Math.Abs(context.ReadValue<float>() - 1f) < 0.5f)
        {
            _characterController.JumpInitiate();
        }
        else
        {
            _characterController.JumpEnd();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        ExecuteActiveInteraction();
    }

    public void OnToggleMap(InputAction.CallbackContext context)
    {
        _mapToggler.Toggle();
    }

    public void AllowInteraction(Interactable interactable) => _interactables.Add(interactable);

    public void DisallowInteraction(Interactable interactable)
    {
        if (interactable == _activeInteractable)
        {
            _activeInteractable.Deactivate();
            _activeInteractable = null;
            ActiveInteractableLocked = false;
        }
        _interactables.Remove(interactable);
    }

    private void ExecuteActiveInteraction() => _activeInteractable?.Interact();

    private void UpdateActiveInteractable()
    {
        Interactable closestInteractable = _activeInteractable;

        if (!ActiveInteractableLocked)
        {
            float activeInteractableSqrDistance = float.MaxValue;
            float interactableSqrDistance = float.MaxValue;

            foreach (var interactable in _interactables)
            {
                interactableSqrDistance = (interactable.transform.position - transform.position).sqrMagnitude;
                if (interactableSqrDistance < activeInteractableSqrDistance)
                {
                    closestInteractable = interactable;
                    activeInteractableSqrDistance = interactableSqrDistance;
                }
            }
        }

        if (closestInteractable != _activeInteractable)
        {
            _activeInteractable?.Deactivate();
            _activeInteractable = closestInteractable;
        }

        if (_activeInteractable?.Active == false)
        {
            _activeInteractable?.Activate();
        }
    }

    private void Update()
    {
        if (_characterController.IsStill)
        {
            if (_movementTweenFlag != 0)
            {
                _movementTweenFlag = 0;
                // _spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.2f);
            }
            _animation.SetRunning(false);
        }
        else if (_characterController.FacingRight)
        {
            if (_movementTweenFlag != 1)
            {
                _movementTweenFlag = 1;
                // _spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 0.1f);
                _characterSpriteRenderer.flipX = false;
            }
            _animation.SetRunning(true);
        }
        else
        {
            if (_movementTweenFlag != 2)
            {
                _movementTweenFlag = 2;
                // _spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 0.1f);
                _characterSpriteRenderer.flipX = true;
            }
            _animation.SetRunning(true);
        }
        
        UpdateActiveInteractable();

        // var particleSystemEmission = _particleSystem.emission;
        // var rateOverTime = particleSystemEmission.GetBurst(0);
        // if (_characterController.OnSteep || _characterController.OnDownwardSlope || _characterController.IsDashing)
        // {
        //     var main = _particleSystem.main;
        //     main.loop = true;
        //
        //     rateOverTime.minCount = 1;
        //     rateOverTime.maxCount = 1;
        // }
        // else
        // {
        //     var main = _particleSystem.main;
        //            main.loop = false;
        //
        //     rateOverTime.minCount = 5;
        //     rateOverTime.maxCount = 15;
        // }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var camTransition = other.GetComponent<CinemachineTransitionInfo>();
        if (camTransition != null)
        {
            camTransition.OnEnterCameraTransition?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var camTransition = other.GetComponent<CinemachineTransitionInfo>(); //repetition
        if (camTransition != null)
        {
            camTransition.OnExitCameraTransition?.Invoke();
        }
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     // _particleSystem.gameObject.transform.position = other.contacts[0].point;
    //     // _particleSystem.gameObject.transform.rotation =
    //     //     Quaternion.Euler(new Vector3(0f, 0f, other.contacts[0].normal.x * -90f));
    //     // _particleSystem.Play();
    //
    //     OnLand?.Invoke();
    // }

    private void OnCollisionStay2D(Collision2D other)
    {
        _lastCollisionPoint = other.contacts[0].point;
        _lastCollisionNormal = other.contacts[0].normal.x * -90f;
    }

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     // _particleSystem.gameObject.transform.position = _lastCollisionPoint;
    //     // _particleSystem.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _lastCollisionNormal));
    //     // _particleSystem.Play();
    // }


    public enum ControlScheme
    {
        KeyboardAndMouse,
        Gamepad
    }

    public void Pause()
    {
        _pause = true;
    }

    public void UnPause()
    {
        _pause = false;
    }

    public void ToggleBlockMovement()
    {
        _blockMovement = !_blockMovement;
    }

    public void Deactivate()
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    public void Activate()
    {
        GetComponent<PlayerInput>().enabled = true;
    }
}