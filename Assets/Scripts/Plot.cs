using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Plot : MonoBehaviour
{

    [SerializeField] private ParticleSystem splashes;
    

    private AudioSource plotSound;

    private void Start()
    {
        plotSound = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        var gameObject = other.gameObject;
        if (gameObject.CompareTag("Player"))
        {
            splashes.Play();
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 1, 1);
            gameObject.transform.parent = transform;
            plotSound.Play();


        }
    }

    private void OnCollisionExit(Collision other)
    {
        var gameObject = other.gameObject;
        if (gameObject.CompareTag("Player"))
        {
            splashes.Play();
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 1, 1);
            gameObject.transform.parent = null;
            gameObject.transform.localScale = new Vector3(7, 7, 5.8f);
        }
    }
}
