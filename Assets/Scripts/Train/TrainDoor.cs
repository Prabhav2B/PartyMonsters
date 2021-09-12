using System;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainDoor : MonoBehaviour
{
    [SerializeField] private Transform trainDoorTransform;

    [SerializeField] private GameObject _closedSign = null;
    [SerializeField] private GameObject _openSign = null;

    private SceneFadeManager _sceneFadeManager;
    private SceneChangeManager _sceneChangeManager;
    private ItemType _myTicket;

    private SpriteRenderer[] _doorSprites;
    private Vector3 _spriteATransform;
    private Vector3 _spriteBTransform;

    public static event Action OnInvalidTicket = delegate {};

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

        var train = GetComponentInParent<TrainExterior>();
        
        if(train == null)
            return;
        
        switch (train.LineColor)
        {
            case TrainLineColor.purple:
                _myTicket = ItemType.TicketPurple;
                break;
            case TrainLineColor.green:
                _myTicket = ItemType.TicketGreen;
                break;
            case TrainLineColor.yellow:
                _myTicket = ItemType.TicketYellow;
                break;
            case TrainLineColor.pink:
                _myTicket = ItemType.TicketPink;
                break;
            case TrainLineColor.blue:
                _myTicket = ItemType.TicketBlue;
                break;
            case TrainLineColor.white:
                break;
            case TrainLineColor._null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        if (!PlayerInventory.ContainsItemOfType(_myTicket))
        {
            OnInvalidTicket.Invoke();
            return;
        }

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