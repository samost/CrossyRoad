using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    
    public int score;

    public int record;

    public int balance;

    public static bool GameState = true;
    public static bool IsFirstLaunch = true;

    private void Start()
    {
        _playerMovement.onAddMoney += AddMoney;
        _playerMovement.onAddScore += AddScore;
        _playerMovement.gameOver += GameOver;
        GetData();
    }


    public void SetData()
    {
        PlayerPrefs.SetInt("Record", record);
        PlayerPrefs.SetInt("Balance", balance);
    }

    public void GetData()
    {
        record = PlayerPrefs.GetInt("Record");
        balance = PlayerPrefs.GetInt("Balance");
    }

    private void AddMoney(int money)
    {
        balance += money;
        SetData();
    }

    private void AddScore(int score)
    {
        this.score = score;
        UpdateRecord();
    }

    private void UpdateRecord()
    {
        if (score >= record)
        {
            record = score;
        }
    }

    private void GameOver()
    {
        GameState = false;
        SetData();
    }

}
