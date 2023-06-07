using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsController : MonoBehaviour
{
    public static FruitsController Instance;
    public List<GameObject> fruits = new List<GameObject>();
    public List<FruitSettings> fruitSettings = new List<FruitSettings>();
    public List<Lines> lines;
    public GameObject prefab;
    public List<int> spawnOrder = new List<int>();
    public int fruitAmount = 0;
    [SerializeField] private float _spawnDelay;
    int maxFruitAmount = 30;
    int i = 0;
    private float _nextSpawnTime;

    public bool onSpin = false;
    public bool onGame = true;
    [SerializeField] GameObject bottomObject;

    private float _spawnOffset = 0f;
    private float _totalSpawnRate = 0;

    public double bet;

    public bool autoSpin;
    public int autoSpinAmount;

    public bool activateBonusGame;
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
            if(sessionPayment != 0)
            {
                if (bonusGame)
                {
                    UIManager.Instance.SetSessionWinText(bonusPayment.ToString("#,0.00") + " " + Wallet.Instance.currency);
                }
                else
                {
                    UIManager.Instance.SetSessionWinText(sessionPayment.ToString("#,0.00") + " " + Wallet.Instance.currency);
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

    private void Start()
    {
        CalculateTotalSpawnRate();
        GenerateSpawnOrders();
    }

    void Update()
    {
        if (fruitAmount < maxFruitAmount)
        {
            spawnOrder.Add(i);
            i++;
            fruitAmount++;
            if (i > 5)
                i = 0;
        }

        if (spawnOrder.Count > 0 && Time.time > _nextSpawnTime)
        {
            SpawnNewFruit(spawnOrder[0]);
            spawnOrder.RemoveAt(0);
            SetDelayTime();
            _spawnOffset += 0.5f;
        }
        if (spawnOrder.Count == 0 && !AnyFruitMoving())
            _spawnOffset = 0;

        if (onSpin && fruits.Count == 0)
        {
            fruitAmount = 0;
            onSpin = false;
            bottomObject.SetActive(true);
        }

        if (!AnyFruitMoving() &&  fruits.Count == maxFruitAmount && spawnOrder.Count == 0 && !onSpin)
            CheckMatchingFruits();
    }

    void GenerateSpawnOrders()
    {
        foreach(Lines line in lines)
        {
            line.spawnOrder.Clear();
            for(int a = 0; a < 6; a++)
            {
                if(a == 0 && UnityEngine.Random.Range(0, 2) == 1)
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

    void GenerateSpawnOrder(Lines line, int howMany = 2)
    {
        FruitSettings r = GetRandomFruitBySpawnRate();
        int c = line.spawnOrder.Count;
        if (r.bonus || c > 0 && r.name == line.spawnOrder[c - 1].name)
            howMany = 1;

        for (int a = 0; a < howMany; a++)
        {
            line.spawnOrder.Add(r);
        }
    }

    bool AnyFruitMoving()
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

    public void Spin()
    {
        if (!onSpin && !onGame && !AnyFruitMoving() && !bonusGame && !activateBonusGame)
        {
            bet = Wallet.Instance.bet;
            if (Wallet.Instance.TryRemoveMoney(bet))
            {
                spawnOrder.Clear();
                GenerateSpawnOrders();
                bottomObject.SetActive(false);
                onSpin = true;
                onGame = true;
                UIManager.Instance.ActivateGoodLuckText();
            }
            else if (autoSpin)
            {
                AutoplayWindow.Instance.OpenWindow();
            }
        }
    }

    public void BonusSpin()
    {
        spawnOrder.Clear();
        GenerateSpawnOrders();
        bottomObject.SetActive(false);
        bonusSpinCount--;
        UIManager.Instance.SetBonusLeftText("FREE SPINS LEFT " + bonusSpinCount);
        onSpin = true;
        onGame = true;
    }

    public void SpawnNewFruit(int a)
    {
        GameObject g = Instantiate(prefab, lines[a].lineTransform);
        g.transform.position += new Vector3(0f, _spawnOffset, 0f);
        g.GetComponent<FruitController>().currentLine = a;
        g.GetComponent<FruitController>().SetFruitSettings(lines[a].spawnOrder[0]);
        fruits.Add(g);
        lines[a].spawnOrder.RemoveAt(0);
    }

    void SetDelayTime()
    {
        _nextSpawnTime = Time.time + _spawnDelay;
    }

    private FruitSettings GetRandomFruitBySpawnRate()
    {

        float randomValue = UnityEngine.Random.Range(0, _totalSpawnRate);
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

    private void CalculateTotalSpawnRate()
    {
        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            _totalSpawnRate += fruitSetting.spawnRate;
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
            if(onGame && !activateBonusGame && !bonusGame && pair.Key.name == fruitSettings[9].name && pair.Value >= 4) // fruitSettings[9] = Bonus Sweet
            {
                activateBonusGame = true;
            }
            if (pair.Value >= 8)
            {
                List<GameObject> matchingFruits = GetMatchingFruits(pair.Key);
                ExplodeMatchingFruits(matchingFruits);
                PayMatchingFruits(pair.Key, pair.Value);
                hasMatchingFruits = true;
            }
        }
        if (!hasMatchingFruits && fruits.Count == maxFruitAmount && onGame)
        {
            if (bonusGame)
            {
                FinishBonusSession();
            }
            else
            {
                FinishSession();
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
            sessionPayment += bet * fruit.eightNinePaymentMultiplier;
        }
        else if (amount == 10 || amount == 11)
        {
            sessionPayment += bet * fruit.tenElevenPaymentMultiplier;
        }
        else if (amount >= 12)
        {
            sessionPayment += bet * fruit.twelveAndAbovePaymentMultiplier;
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
            spawnOrder.Add(currentLine);
            GenerateSpawnOrder(lines[currentLine]);
            Destroy(fruit);
        }
    }

    public void StartBonusGame()
    {
        if (activateBonusGame)
        {
            activateBonusGame = false;
            bonusGame = true;
            bonusSpinCount = 10;
            BonusSpin();
        }
        
    }

    void FinishSession()
    {
        onGame = false;

        if(!activateBonusGame && !bonusGame)
            Wallet.Instance.moneyAmount += sessionPayment;

        if(sessionPayment == 0)
            UIManager.Instance.ActivateSpinToWinText();

        if (!activateBonusGame && autoSpin && autoSpinAmount == 0)
        {
            AutoplayWindow.Instance.OpenWindow();
        }

        if (!activateBonusGame && autoSpin)
        {
            autoSpinAmount--;
            UIManager.Instance.autoplayText.text = autoSpinAmount + " LEFT";
            Spin();
        }

        if (activateBonusGame)
        {
            UIManager.Instance.OpenBonusPopup();
            bonusPayment = sessionPayment;
        }
        sessionPayment = 0;
    }

    void FinishBonusSession()
    {
        onGame = false;
        sessionPayment = 0;

        if (bonusSpinCount == 0)
        {
            bonusGame = false;
            UIManager.Instance.CloseBonusLeftText();
            Wallet.Instance.moneyAmount += bonusPayment;
            bonusPayment = 0;
        }

        if (bonusGame)
            BonusSpin();
    }

    public void ActivateAutoSpin(int amount)
    {
        autoSpin = true;
        autoSpinAmount = amount;
        if (!onGame)
        {
            autoSpinAmount--;
            UIManager.Instance.autoplayText.text = autoSpinAmount + " LEFT";
            Spin();
        }
    }
}

[Serializable] public class Lines
{
    public Transform lineTransform;
    [HideInInspector] public List<FruitSettings> spawnOrder = new List<FruitSettings>();
}