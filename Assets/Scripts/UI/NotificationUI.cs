using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[DisallowMultipleComponent]
public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textComponent = null;
    [SerializeField]
    private Image _iconImage = null;
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

    private Queue<(string, Sprite)> _notificationQueue = new Queue<(string, Sprite)>();
    private bool _processingNotifications = false;

    private void Awake()
    {
        _canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        PlayerInventory.OnAddItem += EnqueueItemNotification;
    }

    private void OnDisable()
    {
        PlayerInventory.OnAddItem -= EnqueueItemNotification;
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
            var notification = _notificationQueue.Dequeue();
            DisplayNotification(notification.Item1, notification.Item2);
            yield return FadeInCoroutine();
            yield return new WaitForSeconds(_displayTime);
            yield return FadeOutCoroutine();
        }
        _processingNotifications = false;
    }

    private void DisplayNotification(string message, Sprite icon)
    {
        _textComponent.text = message;
        _iconImage.sprite = icon;
    }

    private void EnqueueItemNotification(ItemData item)
    {
        string message = string.Format("Received {0}!", item.itemName);
        Sprite icon = item.itemIcon;
        _notificationQueue.Enqueue((message, icon));
        StartProcessingNotifications();
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
