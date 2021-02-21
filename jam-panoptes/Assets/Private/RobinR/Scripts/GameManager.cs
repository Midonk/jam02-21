﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Parameters")]
    public float GameWonTime = 10.0f;
    public float MaxCheeseAmount = 10.0f;

    [Header("Hiddens")]
    private TurretController[] Turrets;
    private bool InGame = false;
    private bool InPause = false;
    private int _AliveTurretsAmount;
    private float _GameWonTimer;
    private float _CheeseAmount;

    private int AliveTurretsAmount{
        get{return _AliveTurretsAmount;}
        set{
            _AliveTurretsAmount = value;
            OnAliveTurretsAmountsChange?.Invoke(_AliveTurretsAmount);
        }
    }
    private float GameWonTimer{
        get{return _GameWonTimer;}
        set{
            _GameWonTimer = value;
            OnWonTimerChange?.Invoke(_GameWonTimer);
        }
    }
    private float CheeseAmount{
        get{return _CheeseAmount;}
        set{
            _CheeseAmount = value;
            OnCheeseAmountChange?.Invoke(_CheeseAmount);
        }
    }

    public delegate void AliveTurretsAmountChangeHandler(int next);
    public event AliveTurretsAmountChangeHandler OnAliveTurretsAmountsChange;
    public delegate void WonTimerChangeHandler(float next);
    public event WonTimerChangeHandler OnWonTimerChange;
    public delegate void CheeseAmountChangeHandler(float next);
    public event CheeseAmountChangeHandler OnCheeseAmountChange;
    
    private void TurretController_OnDeath(bool next)
    {
        GameOver(false);
    }

    public void AddCheese(float amount)
    {
        CheeseAmount = Mathf.Clamp(CheeseAmount + amount, 0, MaxCheeseAmount);
    }
    private void GameOver(bool won)
    {
        Debug.Log(won? "Gagné" : "Perdu");
        SetPause(true);
        HUDManager.Instance.ShowPanel(won? 4 : 5);
        InGame = false;
    }

    public void SetPause(bool isPaused)
    {
        InPause = isPaused;
        Time.timeScale = InPause ? 0 : 1;
        HUDManager.Instance.ShowPanel(InPause? 3 : 2);
    }

    public void CancelGame()
    {
        SetPause(false);
        InGame = false;
        HUDManager.Instance.ShowPanel(0);
    }

    public void NewGame()
    {
        SetPause(false);
        InGame = true;

        GameObject[] turretObjects = GameObject.FindGameObjectsWithTag("Turret");
        Turrets = new TurretController[turretObjects.Length];

        for(int i = 0; i < Turrets.Length; i++)
        {
            Turrets[i] = turretObjects[i].GetComponent<TurretController>();
            
            Turrets[i].OnDeath += TurretController_OnDeath;
        }

        AliveTurretsAmount = Turrets.Length;
        GameWonTimer = 0;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        HUDManager.Instance.ShowPanel(0);
        NewGame();
    }

    private void Update()
    {
        if(InGame && GameWonTimer < GameWonTime && CheeseAmount == 0)
        {
            GameWonTimer += Time.deltaTime;

            if(GameWonTimer >= GameWonTime)
            {
                GameOver(true);
            }
        }else if(InGame && CheeseAmount > 0)
        {
            AddCheese(- Time.deltaTime);
        }

        if(InGame && InputManager.Instance.PauseDown)
        {
            SetPause(!InPause);
        }
    }
}
