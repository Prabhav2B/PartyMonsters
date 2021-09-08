using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrainRattle : MonoBehaviour
{
    void Start()
    {
        Shake();
    }

    // Update is called once per frame
    void Shake()
    {
        transform.DOShakePosition(Random.Range(.5f, 2f), Random.Range(.05f, .1f)).OnComplete(Wait);
    }

    void Wait()
    {
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(.5f, 1.5f));
        Shake();
    }
}
