using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip walkAudio;
    [SerializeField] private AudioClip landAudio;

    [SerializeField] AudioSource _audioSourceOneShot;
    [SerializeField] AudioSource _audioSourceLooper;

    private CharController _characterController;
    private PlayerManager _playerManager;

    private bool walkAudioLock;
    
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _characterController = GetComponent<CharController>();
        _audioSourceOneShot.spread = 1f;
        
        _characterController.OnJump += PlayJumpAudio;
        _playerManager.OnLand += PlayLandAudio;
    }

    private void Update()
    {
        // if (_playerManager.ReceivedInput.magnitude > 0f && !walkAudioLock && _characterController.OnGround)
        // {
        //     walkAudioLock = true;
        //     PlayWalkAudio();
        // }
        // else if (Mathf.Approximately(_playerManager.ReceivedInput.magnitude, 0f) || !_characterController.OnGround && walkAudioLock )
        // {
        //     walkAudioLock = false;
        //     ClearAudioSource();
        // }

    }

    public void PlayJumpAudio()
    {
        _audioSourceOneShot.PlayOneShot(jumpAudio);
    }
    
    
    public void PlayLandAudio()
    {
        _audioSourceOneShot.PlayOneShot(landAudio);
    }

    public void PlayWalkAudio()
    {
        _audioSourceLooper.loop = true;
        _audioSourceLooper.clip = walkAudio;
        _audioSourceLooper.Play();
    }

    public void ClearAudioSource()
    {
        _audioSourceLooper.Pause();
        _audioSourceLooper.loop = false;
    }
    
    public void PauseWalkAudio()
    {
        _audioSourceLooper.Pause();
    }
}
