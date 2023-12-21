using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public List<GameObject> fruits = new List<GameObject>();
    
    public bool onGame = true;
    [SerializeField] GameObject bottomObject;
    bool destroyBottom;

    public double bet;

    public bool readyToSpawn;

    int bonusAmount = 0;
    
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

    void FixedUpdate()
    {
        if (fruits.Count == 0)
        {
            bottomObject.SetActive(true);
            readyToSpawn = true;
        }

        if (onGame && IsEveryFruitOnReel())
            CheckMatchingFruits();

        if (destroyBottom)
        {
            if(bottomObject.transform.position.x >= 18)
            {
                destroyBottom = false;
                bottomObject.SetActive(false);
                bottomObject.transform.position -= new Vector3(15, 0, 0);
            }
            else
            {
                bottomObject.transform.position += new Vector3(1, 0, 0);
            }
        }
    }

    public bool IsEveryFruitOnReel()
    {
        if (fruits.Count == SessionController.Instance.maxFruitAmount && FruitSpawner.Instance.spawnOrder.Count == 0)
        {
            foreach (var fruit in fruits)
            {
                Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
                if (rb == null || rb.velocity.magnitude > 0f || fruit.transform.position.y > 4.5)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public void Spin()
    {
        if (!onGame && !SessionController.Instance.bonusGame && !SessionController.Instance.activateBonusGame)
        {
            bet = Wallet.Instance.bet;
            if (Wallet.Instance.TryRemoveMoney(bet))
            {
                DestroyBottomObject();
                onGame = true;
                readyToSpawn = false;
                SessionController.Instance.StartNewSession();
                UIManager.Instance.ActivateGoodLuckText();
            }
            else if (AutoplayController.Instance.autoSpin)
            {
                AutoplayController.Instance.autoSpin = false;
                AutoplayWindow.Instance.OpenWindow();
            }
        }
    }

    public void BonusSpin()
    {
        UIManager.Instance.tumbleGameObject.SetActive(false);
        DestroyBottomObject();
        SessionController.Instance.bonusSpinCount--;
        UIManager.Instance.SetBonusLeftText("FREE SPINS LEFT " + SessionController.Instance.bonusSpinCount);
        onGame = true;
        readyToSpawn = false;
        SessionController.Instance.StartNewSession();
    }

    public void BuySpin()
    {
        if (!onGame && !SessionController.Instance.bonusGame && !SessionController.Instance.activateBonusGame)
        {
            bet = Wallet.Instance.bet;
            double buyCost = Wallet.Instance.GetBonusBuyCost();
            if (Wallet.Instance.TryRemoveMoney(buyCost))
            {
                DestroyBottomObject();
                onGame = true;
                readyToSpawn = false;
                SessionController.Instance.bonusBuyFeature = true;
                SessionController.Instance.StartNewSession();
                UIManager.Instance.ActivateGoodLuckText();
            }
        }
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
            if (onGame && SessionController.Instance.bonusGame && pair.Key.name == SessionController.Instance.fruitSettings[9].name && pair.Value >= 3)
            {
                SessionController.Instance.addSpin = true;
            }
            if (pair.Value >= 8 && pair.Key.name != SessionController.Instance.fruitSettings[9].name && pair.Key.name != SessionController.Instance.fruitSettings[10].name)
            {
                List<GameObject> matchingFruits = GetMatchingFruits(pair.Key);
                ExplodeMatchingFruits(matchingFruits);
                PayMatchingFruits(pair.Key, pair.Value);
                hasMatchingFruits = true;
            }
            if(pair.Key.name == SessionController.Instance.fruitSettings[9].name)
                bonusAmount = pair.Value;
        }
        if (!hasMatchingFruits && onGame)
        {
            if(bonusAmount >= 4)
                PayMatchingBonuses(SessionController.Instance.fruitSettings[9], bonusAmount);

            bonusAmount = 0;
            onGame = false;
            SessionController.Instance.FinishSession();
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

    void PayMatchingBonuses(FruitSettings fruit, int amount)
    {
        if (amount == 4)
        {
            SessionController.Instance.sessionPayment += bet * fruit.eightNinePaymentMultiplier;
        }
        else if (amount == 5)
        {
            SessionController.Instance.sessionPayment += bet * fruit.tenElevenPaymentMultiplier;
        }
        else if (amount >= 6)
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
            int currentColumn = fruit.GetComponent<FruitController>().currentColumn;
            fruit.GetComponent<FruitController>().Deactive();
            FruitSpawner.Instance.CreateSpawnOrder(currentColumn);
            SessionController.Instance.GenerateSpawnOrder(SessionController.Instance.columns[currentColumn]);
        }
    }
    void DestroyBottomObject()
    {
        destroyBottom = true;
    }
}
