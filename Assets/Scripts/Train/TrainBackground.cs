using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrainBackground : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    
    [SerializeField] private Vector3 leftPosition;
    [SerializeField] private Vector3 rightPosition;
    [SerializeField] private Vector3 centrePosition;

    [SerializeField] private float maxSpeed = 1;

    private SpriteRenderer _spriteA;
    private SpriteRenderer _spriteB;

    private bool _reversing;
    private float _offset;
    private SpriteRenderer[] _sprites;

    private float speed;
    
    public bool Reversing
    {
        set => _reversing = !value;
    }

    private float Direction => _reversing ? 1f : -1f;
    private float ClippingPoint => _reversing ? rightPosition.x : leftPosition.x;

    private void Start()
    {
        speed = maxSpeed;
    }

    private void OnEnable()
    {

        _sprites = GetComponentsInChildren<SpriteRenderer>();

        _spriteA = _sprites[0];
        _spriteB = _sprites[1];
        
        _spriteA.sprite = _sprite;
        _spriteB.sprite = _sprite;

        _offset = centrePosition.x - leftPosition.x;
    }

    public void Halt()
    {
        DOTween.To(() => speed, x => speed = x, 0f, 5f);
    }
    public void UnHalt()
    {
        DOTween.To(() => speed, x => speed = x, maxSpeed, 5f);
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
            if (_reversing)
            {
                if (spr.transform.position.x > ClippingPoint)
                {
                    spr.transform.position += Vector3.left * (_offset * 2);
                }
            }
            else
            {
                if (spr.transform.position.x < ClippingPoint)
                {
                    spr.transform.position += Vector3.right * (_offset * 2);
                }
            }

            spr.transform.position = new Vector3(spr.transform.position.x + (Direction * speed * Time.deltaTime), spr.transform.position.y, spr.transform.position.z);
        }

    }
    
    
}