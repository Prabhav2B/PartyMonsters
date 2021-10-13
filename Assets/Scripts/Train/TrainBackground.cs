using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrainBackground : MonoBehaviour
{
    [SerializeField] private Vector3 leftPosition;
    [SerializeField] private Vector3 rightPosition;
    [SerializeField] private Vector3 centrePosition;

    private Sprite _transitionSprite;
    private Sprite _stationSprite;
    private Sprite _backGroundSprite;
                     
    //Two Sprites that rotate in order to create the
    //Illusion of a infinite background
    private SpriteRenderer _spriteA;
    private SpriteRenderer _spriteB;

    private bool _reversing;
    private float _offset;
    private SpriteRenderer[] _sprites;
    private float _maxSpeed;
    private float _speed;
    private BackgroundState _backgroundFlag;
    private HaltingState _haltingFlag;
    
    public bool Reversing
    {
        set => _reversing = !value;
    }

    private float Direction => _reversing ? 1f : -1f;
    private float ClippingPoint => _reversing ? rightPosition.x : leftPosition.x;

    private void Start()
    {
        _speed = _maxSpeed;
        _backgroundFlag = BackgroundState.background;
        _haltingFlag = HaltingState._null;
    }

    private void OnEnable()
    {

        _sprites = GetComponentsInChildren<SpriteRenderer>();

        _spriteA = _sprites[0];
        _spriteB = _sprites[1];
        
        _spriteA.sprite = _backGroundSprite;
        _spriteB.sprite = _backGroundSprite;

        _offset = centrePosition.x - leftPosition.x;
        _speed = _maxSpeed;
    }

    public void Halt(float timeTillHalt)
    {
        _backgroundFlag = BackgroundState.transition;
        _haltingFlag = HaltingState.halting;
        DOTween.To(() => _speed, x => _speed = x, 0f, timeTillHalt);
    }
    public void UnHalt(float timeTillStart)
    {
        _backgroundFlag = BackgroundState.transition;
        _haltingFlag = HaltingState.starting;
        DOTween.To(() => _speed, x => _speed = x, _maxSpeed, timeTillStart);
    }

    public void Init()
    {
        _spriteA.transform.position = centrePosition;
        _spriteB.transform.position = _reversing ? leftPosition : rightPosition;
    }

    void Update()
    {

        foreach (var spr in _sprites)
        {
            if (  ( _reversing && spr.transform.position.x > ClippingPoint) || ( !_reversing && spr.transform.position.x > ClippingPoint) )
            {
                var direction = _reversing ? Vector3.left : Vector3.right;
                spr.transform.position += direction * (_offset * 2);

                
                if (_haltingFlag != HaltingState._null)
                {
                    switch (_backgroundFlag)
                    {
                        case BackgroundState.background:
                        {
                            // this is not really correct
                            spr.sprite = _backGroundSprite;
                            _backgroundFlag = BackgroundState._null;
                            _haltingFlag = HaltingState._null;
                            break;
                        }
                        case BackgroundState.transition:
                        {
                            spr.sprite = _transitionSprite;
                            _backgroundFlag = _haltingFlag == HaltingState.halting
                                ? BackgroundState.station
                                : BackgroundState.background;
                            break;
                        }
                        case BackgroundState.station:
                        {
                            // this is not really correct
                            spr.sprite = _stationSprite;
                            _backgroundFlag = BackgroundState._null;
                            _haltingFlag = HaltingState._null;
                            break;
                        }
                        case BackgroundState._null:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            var transform1 = spr.transform;
            var position = transform1.position;
            position = new Vector3(position.x + (Direction * _speed * Time.deltaTime), position.y, position.z);
            transform1.position = position;
        }
    }

    public void SetSpriteAndSpeed(Sprite backgroundSprite, float speed)
    {
        _backGroundSprite = backgroundSprite;
        _maxSpeed = speed;
    }

    public void SetTransitionSprite(Sprite transitionSprite)
    {
        _transitionSprite = transitionSprite;
    }
    
    public void SetStationSprite(Sprite stationSprite)
    {
        _stationSprite = stationSprite;
    }


    private enum  BackgroundState
    {
        background,
        transition,
        station,
        _null
    }
    
    private enum  HaltingState
    {
        halting,
        starting,
        _null
    }
}