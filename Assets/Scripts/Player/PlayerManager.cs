using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharController))]
public class PlayerManager : SingleInstance<PlayerManager>
{
    private SpriteRenderer _characterSpriteRenderer;
    private ParticleSystem _particleSystem;
    private Transform _spriteRendererTransform;
    private CharController _characterController;
    private Vector3 _lastCollisionPoint;
    private float _lastCollisionNormal;
    private int _movementTweenFlag;
    private PlayerInput _input;
    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;
    private MainMenu _mainMenu;
    private Vector3 _initalPosition;
    private bool _pause;


    protected Vector2 MoveVector;
    public Vector2 ReceivedInput { get; private set; }
    public bool AtDoor { get; set; }

    public Action OnLand;

    public ControlScheme CurrentControlScheme { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInput>();

        _characterController = GetComponent<CharController>();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();
        _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
        _mainMenu = FindObjectOfType<MainMenu>();

        _characterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRendererTransform = _characterSpriteRenderer.transform;

        //_particleSystem = this.GetComponentInChildren<ParticleSystem>();
        UpdateCurrentScheme(_input.currentControlScheme);

        //Deactivate();
    }

    private void Start()
    {
        _initalPosition = transform.position;
        _movementTweenFlag = -1;
    }

    protected void OnEnable()
    {
        InputUser.onChange += onInputDeviceChange;
    }

    protected void OnDisable()
    {
        InputUser.onChange -= onInputDeviceChange;
    }

    public void ResetScene()
    {
        Deactivate(); //Turn of input from user
        SceneFadeManager.PostFadeOut fadeOutAction = _sceneFadeManager.FadeIn;
        fadeOutAction += Activate;
        _sceneFadeManager.FadeOut(fadeOutAction);
        fadeOutAction = null;
    }


    private void ResetPlayerPosition()
    {
        transform.position = _initalPosition;
    }

    private void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
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
    
    private void OnDoubleClickTest()
    {
        Debug.Log("Double Click!");
    
    }

    private void OnMainMenu(InputValue input)
    {
        _mainMenu.TriggerMainMenu();
    }

    private void OnMove(InputValue input)
    {
        if(_pause)
            return;
        
        ReceivedInput = input.Get<Vector2>();
        _characterController.Move(ReceivedInput);
    }

    private void OnJump(InputValue input)
    {
        if(_pause)
            return;
        
        if (Math.Abs(input.Get<float>() - 1f) < 0.5f)
        {
            _characterController.JumpInitiate();
        }
        else
        {
            _characterController.JumpEnd();
        }
    }

    private void OnInteract(InputValue input)
    {
        ExecuteInteraction();
    }

    private void ExecuteInteraction()
    {
        if (AtDoor)
        {
            _sceneFadeManager.FadeOut(_sceneChangeManager.SwitchToTrainInterior);
        }
    }

    void Update()
    {
        if (_characterController.IsStill)
        {
            if (_movementTweenFlag != 0)
            {
                _movementTweenFlag = 0;
                _spriteRendererTransform.DOLocalRotate(Vector3.zero, 0.2f);
            }
        }
        else if (_characterController.FacingRight)
        {
            if (_movementTweenFlag != 1)
            {
                _movementTweenFlag = 1;
                _spriteRendererTransform.DOLocalRotate(Vector3.forward * -5f, 0.1f);
                _characterSpriteRenderer.flipX = false;
            }
        }
        else
        {
            if (_movementTweenFlag != 2)
            {
                _movementTweenFlag = 2;
                _spriteRendererTransform.DOLocalRotate(Vector3.forward * 5f, 0.1f);
                _characterSpriteRenderer.flipX = true;
            }
        }

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

    public void Deactivate()
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    public void Activate()
    {
        GetComponent<PlayerInput>().enabled = true;
    }
}