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

    public bool Reversing
    {
        set => _reversing = !value;
    }

    private float Direction => _reversing ? 1f : -1f;
    private float ClippingPoint => _reversing ? rightPosition.x : leftPosition.x;

    private void Start()
    {
        _speed = _maxSpeed;
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

    
    //Technically the background doesn't really stop, The dummy station runs on top of that
    
    // public void Halt(float timeTillHalt)
    // {
    //     DOTween.To(() => _speed, x => _speed = x, 0f, timeTillHalt);
    // }
    //
    // public void UnHalt(float timeTillStart)
    // {
    //     DOTween.To(() => _speed, x => _speed = x, _maxSpeed, timeTillStart);
    // }

    public void Init()
    {
        _spriteA.transform.position = centrePosition;
        _spriteB.transform.position = _reversing ? leftPosition : rightPosition;
    }

    void Update()
    {
        foreach (var spr in _sprites)
        {
            if (_reversing && spr.transform.position.x > ClippingPoint ||
                !_reversing && spr.transform.position.x < ClippingPoint)
            {
                var direction = _reversing ? Vector3.left : Vector3.right;
                spr.transform.position += direction * (_offset * 2);
            }

            spr.transform.position =
                new Vector3(
                    spr.transform.position.x + (Direction * Mathf.Clamp(_speed, 0f, _maxSpeed) * Time.deltaTime),
                    spr.transform.position.y, spr.transform.position.z);
        }
    }
    
    public void SetSpriteAndSpeed(Sprite backgroundSprite, float speed)
    {
        _backGroundSprite = backgroundSprite;
        _maxSpeed = speed;
    }
    
    private enum BackgroundState
    {
        background,
        transition,
        station,
        _null
    }

    private enum HaltingState
    {
        halting,
        starting,
        _null
    }

    
}