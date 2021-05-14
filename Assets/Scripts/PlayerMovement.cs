using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyPooler;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _yPos;
    private bool _isMoved;

    public int lefLimit = 5;
    public int rightLimit = -5;

    public delegate void AddScore(int score);
    public event AddScore onAddScore;
    
    public delegate void AddMoney(int money);
    public event AddMoney onAddMoney;
    
    public delegate void GameOver();
    public event GameOver gameOver;

    [SerializeField] private ParticleSystem splashes;
    [SerializeField] private SwipeController _swipeController;

    private AudioSource playerSound;

    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField]
    private AudioClip moneySound;
    [SerializeField]
    private AudioClip waterDeathSound;
    [SerializeField]
    private AudioClip carDeathSound;

    private ObjectPooler Pooler;

    private float _deathStayTime = 3f;
    private float timer = 0;

    [SerializeField] private GameObject lisPrefab;

    private bool _isJunping = false;
    [SerializeField] private ParticleSystem shoot;
    [SerializeField] private AudioClip shootSound;
    

    private void Start()
    {
        _yPos = transform.position.y;
        playerSound = GetComponent<AudioSource>();
        Pooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _deathStayTime)
        {
            LisEvent();
            timer = 0;
        }
        
        _isMoved = SwipeController.tap ? true : false;
        
        if (_isMoved && !_isJunping) 
        {
            Move(SwipeController.direction);
            timer = 0;

        }
        
        CheckPosition();
        
        
        
    }

    private void Move(Vector3 direction)
    {
        Vector3 newPos = transform.position + direction;

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f))
        {
            if(hit.collider.CompareTag("Obstacle"))
                return;
        }

        
        
        
        
        if (newPos.x <= lefLimit || newPos.x >= rightLimit) return;


        newPos.y = _yPos;

        transform.DOLookAt(newPos, 0.2f, AxisConstraint.Y);

        float checkX = newPos.x;
        float checkZ = newPos.z;
        if (checkX % Math.Round(checkX) != 1)
            newPos.x = (float) Math.Round(checkX);
        
        if (checkZ % Math.Round(checkZ) != 1)
            newPos.z = (float) Math.Round(checkZ);


        transform.DOJump(newPos, 1f, 1, 0.2f).OnStart((() => _isJunping = true)).OnComplete(() => _isJunping = false);
        playerSound.PlayOneShot(jumpSound);
        
        onAddScore.Invoke((int)newPos.z);
    }

    private void SetMoved()
    {
        _swipeController.enabled = true;
        _isMoved = false;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Money"))
        {
            Pooler.ReturnToPool("coin", other.gameObject);
            onAddMoney.Invoke(1);
            playerSound.PlayOneShot(moneySound);
        }
        if (other.gameObject.CompareTag("Car"))
        {
            GameController.IsFirstLaunch = true;
            gameOver.Invoke();
            transform.DOScaleY(0.5f, 0f).OnComplete(() => _swipeController.enabled = false);
            playerSound.PlayOneShot(carDeathSound);
        }
        if (other.gameObject.CompareTag("LIS"))
        {
            GameController.IsFirstLaunch = true;
            transform.DOScaleY(0.5f, 0f);
            gameOver.Invoke();
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        var gameObject = other.gameObject;
        if (gameObject.CompareTag("Water"))
        {
            _swipeController.enabled = false;
            GameController.IsFirstLaunch = true;
            WaterDeath();
        }
    }

    private void WaterDeath()
    {
        gameOver.Invoke();
        transform.DOMoveY(-1, 0.6f);
        transform.DOShakeScale(0.5f, Vector3.one * 3, 1, 0, true).OnComplete(() => this.gameObject.SetActive(false));
        splashes.Play();
        playerSound.PlayOneShot(waterDeathSound);
    }

    private void CheckPosition()
    {
        if (transform.position.x < lefLimit)
        {
            WaterDeath();
        }
        if (transform.position.x > rightLimit)
        {
            WaterDeath();
        }
    }

    private void LisEvent()
    {
        _swipeController.enabled = false;
        var o = Instantiate(lisPrefab, new Vector3(10f, 0.8f, transform.position.z), Quaternion.Euler(0,90,0));
        o.transform.DOMoveX(-10, 2f);
    }

    public void Shoot()
    {
        _isJunping = true;
        shoot.Play();
        playerSound.PlayOneShot(shootSound);
        Ray ray = new Ray(transform.localPosition, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if(hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Car"))
                hit.collider.gameObject.SetActive(false);
        }

        Invoke("SetJunp", 0.2f);
    }

    private void SetJunp()
    {
        _isJunping = false;
    }


}
