using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum NotificationType
{
    Item,
    InvalidTicket
}

public struct Notification
{
    public NotificationType Type;
    public string Message;
    public Sprite Icon;

    public Notification(NotificationType type, string message, Sprite icon)
    {
        Type = type;
        Message = message;
        Icon = icon;
    }
}

[DisallowMultipleComponent]
public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textComponent = null;
    [SerializeField]
    private Image _iconImage = null;
    [SerializeField]
    private Sprite _invalidTicketSprite = null;
    [SerializeField]
    private CanvasGroup _canvasGroup = null;
    [
        SerializeField,
        Min(0f)
    ]
    private float _displayTime = 0f;
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeInDuration = 0f;
    [
        SerializeField,
        Min(0f)
    ]
    private float _fadeOutDuration = 0f;

    private Queue<Notification> _notificationQueue = new Queue<Notification>();
    private bool _processingNotifications = false;
    private Notification _currentNotification = default(Notification);

    private void Awake()
    {
        _canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        PlayerInventory.OnAddItem += EnqueueItemNotification;
        TrainDoor.OnInvalidTicket += EnqueueInvalidTicketNotification; 
    }

    private void OnDisable()
    {
        PlayerInventory.OnAddItem -= EnqueueItemNotification;
        TrainDoor.OnInvalidTicket -= EnqueueInvalidTicketNotification; 
    }

    private void StartProcessingNotifications()
    {
        if (!_processingNotifications)
        {
            StartCoroutine(ProcessNotificationQueue());
        }
    }

    private IEnumerator ProcessNotificationQueue()
    {
        _processingNotifications = true;
        while (_notificationQueue.Count > 0)
        {
            _currentNotification = _notificationQueue.Dequeue();
            DisplayNotification(_currentNotification);
            yield return FadeInCoroutine();
            yield return new WaitForSeconds(_displayTime);
            yield return FadeOutCoroutine();
        }
        _currentNotification = default(Notification);
        _processingNotifications = false;
    }

    private void DisplayNotification(Notification notification)
    {
        _textComponent.text = notification.Message;
        _iconImage.sprite = notification.Icon;
    }

    private void EnqueueItemNotification(ItemData item)
    {
        var type = NotificationType.Item;
        string message = string.Format("Received {0}!", item.itemName);
        Sprite icon = item.itemIcon;
        _notificationQueue.Enqueue(new Notification(type, message, icon));
        StartProcessingNotifications();
    }

    private void EnqueueInvalidTicketNotification()
    {
        if (!(_processingNotifications && _currentNotification.Type == NotificationType.InvalidTicket))
        {
            var type = NotificationType.InvalidTicket;
            string message = "You don't have a valid ticket for this line!";
            Sprite icon = _invalidTicketSprite;
            _notificationQueue.Enqueue(new Notification(type, message, icon));
            StartProcessingNotifications();
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return FadeFromTo(0f, 1f, _fadeInDuration);
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return FadeFromTo(1f, 0f, _fadeOutDuration);
    }

    private IEnumerator FadeFromTo(float from, float to, float duration)
    {
        _canvasGroup.alpha = from;
        var fade = _canvasGroup.DOFade(to, duration);
        yield return fade.WaitForCompletion();
    }
}
