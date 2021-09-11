using System;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
    [SerializeField] private Transform trainDoorTransform;
    
    [SerializeField]
    private GameObject _closedSign = null;
    [SerializeField]
    private GameObject _openSign = null;

    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;

    private SpriteRenderer[] _doorSprites;
    private Vector3 _spriteATransform;
    private Vector3 _spriteBTransform;


    private void Awake()
    {
        _sceneChangeManager = FindObjectOfType<SceneChangeManager>();
        _sceneFadeManager = FindObjectOfType<SceneFadeManager>();

        _doorSprites = trainDoorTransform.GetComponentsInChildren<SpriteRenderer>();

        if (_doorSprites.Length > 0)
        {
            _spriteATransform = _doorSprites[0].transform.localPosition;
            _spriteBTransform = _doorSprites[1].transform.localPosition;
        }
    }

    public void SetDoorSprite(Sprite sprite)
    {
        foreach (var door in _doorSprites)
        {
            door.sprite = sprite;
        }
    }
    

    public void EnterTrain()
    {
        SceneFadeManager.PostFadeOut f = () => { };
        f += _sceneChangeManager.SwitchToTrainInterior;
        
        _sceneFadeManager.FadeOut(f);
    }
    
    public void ExitTrain()
    {
        SceneFadeManager.PostFadeOut f = () => { };
        f += _sceneChangeManager.SwitchToStation;
        
        _sceneFadeManager.FadeOut(f);
    }

    public void OnDoorOpen()
    {
        _closedSign.SetActive(false);
        _openSign.SetActive(true);
    }

    public void OnDoorClose()
    {
        _closedSign.SetActive(true);
        _openSign.SetActive(false);
    }

    public void ResetDoors()
    {
        if (_doorSprites != null)
        {
            _doorSprites[0].transform.localPosition = _spriteATransform;
            _doorSprites[1].transform.localPosition = _spriteBTransform;
        }

    }

    public void OpenDoorAnim()
    {
        _doorSprites[0].transform.DOLocalMoveX(_doorSprites[0].transform.localPosition.x - 2.5f, 1f);
        _doorSprites[1].transform.DOLocalMoveX(_doorSprites[1].transform.localPosition.x + 2.5f, 1f);
    }

    public void CloseDoorAnim()
    {
        _doorSprites[0].transform.DOLocalMoveX(_doorSprites[0].transform.localPosition.x + 2.5f, 1f);
        _doorSprites[1].transform.DOLocalMoveX(_doorSprites[1].transform.localPosition.x - 2.5f, 1f);
    }



}
