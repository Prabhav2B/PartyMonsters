using DG.Tweening;
using UnityEngine;

public class TrainArrival : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 destinationPosition;

    
    void Start()
    {
        transform.DOLocalMove(destinationPosition, 12f);
    }

}
