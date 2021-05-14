using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{

    private float _state;

    [SerializeField] private Sprite soundImageOn;
    [SerializeField] private Sprite soundImageOff;
    private Image _image;

    private void Awake()
    {
        _state = AudioListener.volume;
    }

    private void Start()
    {
        _image = GetComponent<Image>();
        _image.sprite = _state >= 1 ? soundImageOn : soundImageOff;
    }

    public void SoundControll()
    {
        if (_state == 0)
        {
            _state = 1;
            _image.sprite = soundImageOn;
        }
        else
        {
            _state = 0;
            _image.sprite = soundImageOff;
        }
        AudioListener.volume = _state;
    }
}
