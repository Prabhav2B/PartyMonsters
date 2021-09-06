using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class CinemachineTransitionInfo : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera toCamera;

    public UnityAction OnEnterCameraTransition;
    public UnityAction OnExitCameraTransition;


    private CinemachineVirtualCamera fromVCam;
    private CinemachineBrain brain;

    private void Awake()
    {
        OnEnterCameraTransition += EntryTransition;
        OnExitCameraTransition += ExitTransition;
    }

    private void Start()
    {
        brain = CinemachineCore.Instance.GetActiveBrain(0);
    }

    private void EntryTransition()
    {
        fromVCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        fromVCam.gameObject.SetActive(false);
        toCamera.gameObject.SetActive(true);
    }

    private void ExitTransition()
    {
        toCamera.gameObject.SetActive(false);
        fromVCam.gameObject.SetActive(true);
    }
}