using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

public class CharController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f, maxGravitySpeed = 10f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;
    [SerializeField, Range(-50f, 50f)] private float maxGravityAcceleration = -9.8f;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 5.0f;
    [SerializeField] private bool fastFallActive;

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _contactNormal;
    private float _moveVal;
    private float _desiredMovementVelocity, _desiredGravityVelocity;
    private float _jumpVelocity;
    private bool _desiredJump;
    private bool _jumpBuffer, _coyoteJump;
    private bool _onGround, _onDownwardSlope;
    private bool _fastFall;
    private bool _facingRight, _isStill;
    private int _jumpPhase;
    private int _stepsSinceLastJump, _stepsSinceJumpBuffer, _stepsSinceCoyoteFlag;
    private float _minGroundDotProduct;
    private int _maxAirJumps = 0;


    private const int JumpBufferFrames = 8;
    private const int CoyoteFlagFrames = 10;

    public bool JumpMaxed => (_jumpPhase > _maxAirJumps || (!_onGround && _maxAirJumps == 0));
    public bool IsStill => _isStill;
    public bool FacingRight => _facingRight;
    public bool OnGround => _onGround;
    public bool OnDownwardSlope => _onDownwardSlope;


    public Action OnJump;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        OnValidate();
    }

    private void Start()
    {
        _facingRight = true;
    }

    public void Move(Vector2 moveVector)
    {
        _moveVal = Mathf.Round(moveVector.x); // Lock this while wall jumping
    }

    public void JumpInitiate()
    {
        _desiredJump = true;
    }

    public void JumpEnd()
    {
        _fastFall = true;
    }
    
    private void Update()
    {
        _isStill = false;
        _desiredMovementVelocity = _moveVal * maxSpeed;
        _desiredGravityVelocity = -maxGravitySpeed;

        if (_velocity.x > maxSpeed * 0.75f)
        {
            _facingRight = true;
        }
        else if (_rb.velocity.x < maxSpeed * -0.75f)
        {
            _facingRight = false;
        }
        else 
        {
            _isStill = true;
        }
    }

    private void FixedUpdate()
    {

        UpdateState();
        
        if (_desiredJump || _jumpBuffer)
        {
            _desiredJump = false;
            
            if (_jumpPhase > _maxAirJumps) //check for steep proximity here itself
            {
                if (!_jumpBuffer)
                {
                    _jumpBuffer = true;
                    _stepsSinceJumpBuffer = 0;
                }
            }
            else if(_jumpPhase <= _maxAirJumps )
            {
                _jumpBuffer = false;
                Jump();
            }
            else
            {
                if (!_jumpBuffer)
                {
                    _jumpBuffer = true;
                    _stepsSinceJumpBuffer = 0;
                }
            }
        }

        if (_fastFall && fastFallActive && !_jumpBuffer)
        {
            if (_velocity.y >= _jumpVelocity / 3f)
                _velocity.y += (-1f * _jumpVelocity) / 4f;

            _fastFall = false;
        }


        AdjustVelocity();

        float maxGravityChange;

        if (!_onGround && _velocity.y < 0f)
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            maxGravityChange = -maxGravityAcceleration * Time.fixedDeltaTime;
        }

        _velocity.y = Mathf.MoveTowards(_velocity.y, _desiredGravityVelocity, maxGravityChange);
        
        _velocity.x = Mathf.Min(_velocity.x, 30f);
        _velocity.y = Mathf.Min(_velocity.y, 30f);

        _rb.velocity = _velocity;

        ClearState();
    }

    private void UpdateState()
    {
        _velocity = _rb.velocity;
        _stepsSinceLastJump++;
        _stepsSinceJumpBuffer++;
        _stepsSinceCoyoteFlag++;


        if (_stepsSinceJumpBuffer > JumpBufferFrames && _jumpBuffer)
        {
            _jumpBuffer = false;
        }

        if (_stepsSinceCoyoteFlag > CoyoteFlagFrames)
        {
            _coyoteJump = false;
        }

        if (!_onGround)
        {
            _contactNormal = Vector2.up;
            return;
        }

        _contactNormal.Normalize();

        if (_stepsSinceLastJump > 1)
        {
            _jumpPhase = 0;
        }
    }

    void AdjustVelocity()
    {
        Vector2 xAxis = Vector2.right.normalized;

        float currentX = Vector2.Dot(_velocity, xAxis);


        float acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        if (Mathf.Approximately(_desiredMovementVelocity, 0f) &&
            !_onGround) //come to stop fast when control is released mid air
            maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredMovementVelocity, maxSpeedChange);

        _velocity += xAxis * (newX - currentX);

        float slopeFactor = Mathf.Abs(Vector2.Angle(_contactNormal, Vector2.up)) / 90f;


        if (_velocity.y < -0.1 && _onGround && !_coyoteJump) // little hack to adjust speed on slopes
        {
            _velocity += _velocity.normalized * (slopeFactor * 1.5f);
            _onDownwardSlope = true;
        }
        else if (_velocity.y > 0.1 && _onGround && !_coyoteJump)
        {
            _velocity -= _velocity.normalized * slopeFactor;
            _onDownwardSlope = false;
        }
        else
        {
            _onDownwardSlope = false;
        }
    }

    private void Jump()
    {
        if (_coyoteJump)
        {
            _coyoteJump = false;
        }

        _stepsSinceLastJump = 0;

        if (_jumpPhase == 0 && !_onGround)
        {
            _jumpPhase = 1;
        }

        _jumpPhase++;
        _jumpVelocity = Mathf.Sqrt(-2f * maxGravityAcceleration * jumpHeight);


        if (!Mathf.Approximately(_contactNormal.y, 1f) && _velocity.y > 0) //check if on a upward slope
        {
            _velocity.y += ((_jumpVelocity));
        }
        else
        {
            _velocity.y = (_jumpVelocity + 1.5f);
        }
        
        switch (_jumpPhase)
        {
            case 1:
                OnJump?.Invoke();
                break;
        }


    }
    

    public void ClearState()
    {
        _onGround = _coyoteJump;
        _contactNormal = Vector2.zero;
    }

    public void ClampVelocity()
    {
        _velocity = _rb.velocity;
        _velocity.x = Mathf.Min(_velocity.x, 10f);
        _velocity.y = Mathf.Min(_velocity.y, 10f);

        _rb.velocity = _velocity;
    }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EvaluateCollision(other);
        _coyoteJump = false; //careful about this here
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EvaluateCollision(other);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        EvaluateCollision(other);
        _coyoteJump = !_onGround && _jumpPhase == 0;
        _stepsSinceCoyoteFlag = 0;
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.contacts[i].normal;


            if ((normal.y >= _minGroundDotProduct))
            {
                _onGround = true;
                _contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
            }
        }

        if (_onGround) // a hack
        {
        }
    }
    
}