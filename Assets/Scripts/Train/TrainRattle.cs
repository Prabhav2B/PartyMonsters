using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrainRattle : MonoBehaviour
{
    public bool Stopped { get; set; }

    void Start()
    {
        Shake();
    }

    // Update is called once per frame
    void Shake()
    {
        transform.DOShakePosition(Random.Range(.3f, 1.5f), Random.Range(.01f, .05f)).OnComplete(Wait);
    }

    void Wait()
    {
        if(gameObject.activeSelf)
            StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        while (true)
        {
            if (!Stopped)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(Random.Range(.75f, 2f));
        Shake();
    }
}
