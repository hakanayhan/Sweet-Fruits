using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionController : MonoBehaviour
{
    public static SessionController Instance;
    public List<FruitSettings> fruitSettings = new List<FruitSettings>();
    public List<Lines> lines;

    public int fruitAmount = 0;
    public int maxFruitAmount = 30;

    public bool activateBonusGame;
    public bool addSpin;
    public bool bonusGame;
    public int bonusSpinCount;
    public double bonusPayment;

    private double _sessionPayment;
    public double sessionPayment
    {
        get { return _sessionPayment; }
        set
        {
            if (bonusGame && value != 0)
                bonusPayment += value - _sessionPayment;
            _sessionPayment = value;
            if (sessionPayment != 0)
            {
                if (bonusGame)
                {
                    UIManager.Instance.SetTumbleText(Wallet.Instance.currency + sessionPayment.ToString("#,0.00"));
                    UIManager.Instance.SetSessionWinText(Wallet.Instance.currency + bonusPayment.ToString("#,0.00"));
                }
                else
                {
                    UIManager.Instance.SetSessionWinText(Wallet.Instance.currency + sessionPayment.ToString("#,0.00"));
                }

            }
        }
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartNewSession()
    {
        fruitAmount = 0;
        sessionPayment = 0;

        FruitSpawner.Instance.spawnOrder.Clear();
        GenerateSpawnOrders();
        for (int i = 0; fruitAmount < maxFruitAmount; fruitAmount++)
        {
            FruitSpawner.Instance.spawnOrder.Add(i);
            i++;
            if (i > 5)
                i = 0;
        }
    }

    public void FinishSession()
    {
        if (!activateBonusGame && !bonusGame)
            Wallet.Instance.moneyAmount += sessionPayment;

        if (sessionPayment == 0)
            UIManager.Instance.ActivateSpinToWinText();

        if (!activateBonusGame)
            AutoplayController.Instance.CheckAutoSpin();

        if (activateBonusGame)
        {
            UIManager.Instance.OpenBonusPopup();
            bonusPayment = sessionPayment;
        }
        sessionPayment = 0;
    }

    public void FinishBonusSession()
    {
        int sessionMultiplier = 0;
        if (sessionPayment != 0)
        {
            foreach (GameObject fruit in GameController.Instance.fruits)
            {
                FruitController fruitController = fruit.GetComponent<FruitController>();
                FruitSettings settingsComponent = fruitController.GetFruitSettings();
                if (settingsComponent.name == fruitSettings[10].name)
                {

                    sessionMultiplier += fruitController.multiplier;
                }
            }
        }

        if (sessionMultiplier > 0)
        {
            double oldPayment = sessionPayment;
            sessionPayment *= sessionMultiplier;
            UIManager.Instance.SetTumbleText(Wallet.Instance.currency + oldPayment.ToString("#,0.00") + " X " + sessionMultiplier);
            Invoke("SetTumbleTextAfterMultiply", 0.5f);
        }

        if (addSpin)
        {
            bonusSpinCount += 5;
            addSpin = false;
        }

        if (bonusSpinCount == 0)
        {
            bonusGame = false;
            UIManager.Instance.CloseBonusLeftText();
            Wallet.Instance.moneyAmount += bonusPayment;
            bonusPayment = 0;
            UIManager.Instance.tumbleGameObject.SetActive(false);
        }
        if (bonusGame)
            Invoke("BonusSpin", 1f);
    }

    void BonusSpin()
    {
        GameController.Instance.BonusSpin();
    }
    void GenerateSpawnOrders()
    {
        foreach (Lines line in lines)
        {
            line.spawnOrder.Clear();
            for (int a = 0; a < 6; a++)
            {
                if (a == 0 && UnityEngine.Random.Range(0, 2) == 1)
                {
                    GenerateSpawnOrder(line, 1);
                }
                else
                {
                    GenerateSpawnOrder(line);
                }

            }
        }
    }

    public void GenerateSpawnOrder(Lines line, int howMany = 2)
    {
        FruitSettings r = GetRandomFruitBySpawnRate();
        int c = line.spawnOrder.Count;
        if (r.bonus || r.name == fruitSettings[10].name || c > 0 && r.name == line.spawnOrder[c - 1].name)
            howMany = 1;

        for (int a = 0; a < howMany; a++)
        {
            line.spawnOrder.Add(r);
        }
    }

    private FruitSettings GetRandomFruitBySpawnRate()
    {

        float randomValue = UnityEngine.Random.Range(0, CalculateTotalSpawnRate());
        float cumulativeSpawnRate = 0;

        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            cumulativeSpawnRate += (bonusGame) ? fruitSetting.bonusSpawnRate : fruitSetting.spawnRate;

            if (randomValue < cumulativeSpawnRate)
            {
                return fruitSetting;
            }
        }
        return fruitSettings[0];
    }

    private float CalculateTotalSpawnRate()
    {
        float totalSpawnRate = 0;
        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            totalSpawnRate += (bonusGame) ? fruitSetting.bonusSpawnRate : fruitSetting.spawnRate;
        }
        return totalSpawnRate;
    }

    public void StartBonusGame()
    {
        if (activateBonusGame)
        {
            activateBonusGame = false;
            bonusGame = true;
            bonusSpinCount = 10;
            GameController.Instance.BonusSpin();
        }
    }
    void SetTumbleTextAfterMultiply()
    {
        if (bonusGame)
            UIManager.Instance.SetTumbleText(Wallet.Instance.currency + sessionPayment.ToString("#,0.00"));
    }
}

[Serializable] public class Lines
{
    public Transform lineTransform;
    [HideInInspector] public List<FruitSettings> spawnOrder = new List<FruitSettings>();
}