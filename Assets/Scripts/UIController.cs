
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameController _controller;
    

   
    
    public Text currentScore;
    public Text record;
    public Text balance;
    public GameObject gameOverUI;
    
    

    private void Update()
    {
        balance.text = _controller.balance.ToString();
        record.text = _controller.record.ToString();
        currentScore.text = _controller.score.ToString();

        if (!GameController.GameState)
        {
            GameOverUI();
        }
    }

    private void GameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    public void StartGameUI()
    {
        DOTween.KillAll();
        GameController.GameState = true;
        GameController.IsFirstLaunch = true;
        SceneManager.LoadScene("Main");
        GameController.IsFirstLaunch = true;
        

    }
}
