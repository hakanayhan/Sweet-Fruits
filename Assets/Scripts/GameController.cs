using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<GameObject> fruits = new List<GameObject>();
    
    public bool onSpin = false;
    public bool onGame = true;
    [SerializeField] GameObject bottomObject;

    public double bet;

    public bool readyToSpawn;
    
    public List<int> multiplierList = new List<int>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (onSpin && fruits.Count == 0)
        {
            onSpin = false;
            bottomObject.SetActive(true);
            readyToSpawn = true;
        }

        if (!AnyFruitMoving() && !onSpin && AreFruitsOnScreen())
            CheckMatchingFruits();
    }

    public bool AnyFruitMoving()
    {
        foreach (var fruit in fruits)
        {
            Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0f)
            {
                return true;
            }
        }
        return false;
    }

    public bool AreFruitsOnScreen()
    {
        foreach (var fruit in fruits)
        {
            if (fruits.Count == SessionController.Instance.maxFruitAmount && FruitSpawner.Instance.spawnOrder.Count == 0 && fruit.transform.position.y < 5)
            {
                return true;
            }
        }
        return false;
    }

    public void Spin()
    {
        if (!onSpin && !onGame && !AnyFruitMoving() && !SessionController.Instance.bonusGame && !SessionController.Instance.activateBonusGame)
        {
            bet = Wallet.Instance.bet;
            if (Wallet.Instance.TryRemoveMoney(bet))
            {
                bottomObject.SetActive(false);
                onSpin = true;
                onGame = true;
                readyToSpawn = false;
                SessionController.Instance.StartNewSession();
                UIManager.Instance.ActivateGoodLuckText();
            }
            else if (AutoplayController.Instance.autoSpin)
            {
                AutoplayWindow.Instance.OpenWindow();
            }
        }
    }

    public void BonusSpin()
    {
        UIManager.Instance.tumbleGameObject.SetActive(false);
        bottomObject.SetActive(false);
        SessionController.Instance.bonusSpinCount--;
        UIManager.Instance.SetBonusLeftText("FREE SPINS LEFT " + SessionController.Instance.bonusSpinCount);
        onSpin = true;
        onGame = true;
        readyToSpawn = false;
        SessionController.Instance.StartNewSession();
    }

    void CheckMatchingFruits()
    {
        Dictionary<FruitSettings, int> fruitCount = new Dictionary<FruitSettings, int>();

        foreach (GameObject fruit in fruits)
        {
            FruitController fruitController = fruit.GetComponent<FruitController>();
            if (fruitController != null)
            {
                FruitSettings fruitSetting = fruitController.GetFruitSettings();
                if (!fruitCount.ContainsKey(fruitSetting))
                {
                    fruitCount[fruitSetting] = 1;
                }
                else
                {
                    fruitCount[fruitSetting]++;
                }
            }
        }

        bool hasMatchingFruits = false;

        foreach (KeyValuePair<FruitSettings, int> pair in fruitCount)
        {
            if(onGame && !SessionController.Instance.activateBonusGame && !SessionController.Instance.bonusGame && pair.Key.name == SessionController.Instance.fruitSettings[9].name && pair.Value >= 4) // fruitSettings[9] = Bonus Sweet
            {
                SessionController.Instance.activateBonusGame = true;
            }
            if (pair.Value >= 8)
            {
                List<GameObject> matchingFruits = GetMatchingFruits(pair.Key);
                ExplodeMatchingFruits(matchingFruits);
                PayMatchingFruits(pair.Key, pair.Value);
                hasMatchingFruits = true;
            }
        }
        if (!hasMatchingFruits && onGame)
        {
            onGame = false;
            if (SessionController.Instance.bonusGame)
            {
                SessionController.Instance.FinishBonusSession();
            }
            else
            {
                SessionController.Instance.FinishSession();
            }
            
        }
    }

    List<GameObject> GetMatchingFruits(FruitSettings fruitSetting)
    {
        List<GameObject> matchingFruits = new List<GameObject>();
        foreach (GameObject fruit in fruits)
        {
            FruitController fruitController = fruit.GetComponent<FruitController>();
            if (fruitController != null && fruitController.GetFruitSettings() == fruitSetting)
            {
                matchingFruits.Add(fruit);
            }
        }

        return matchingFruits;
    }

    void PayMatchingFruits(FruitSettings fruit, int amount)
    {
        if(amount == 8 || amount == 9)
        {
            SessionController.Instance.sessionPayment += bet * fruit.eightNinePaymentMultiplier;
        }
        else if (amount == 10 || amount == 11)
        {
            SessionController.Instance.sessionPayment += bet * fruit.tenElevenPaymentMultiplier;
        }
        else if (amount >= 12)
        {
            SessionController.Instance.sessionPayment += bet * fruit.twelveAndAbovePaymentMultiplier;
        }
        else
        {
            Debug.Log("payment error");
        }
    }

    void ExplodeMatchingFruits(List<GameObject> matchingFruits)
    {
        foreach (GameObject fruit in matchingFruits)
        {
            int currentLine = fruit.GetComponent<FruitController>().currentLine;
            FruitSpawner.Instance.CreateSpawnOrder(currentLine);
            SessionController.Instance.GenerateSpawnOrder(SessionController.Instance.lines[currentLine]);
            Destroy(fruit);
        }
    }
}
