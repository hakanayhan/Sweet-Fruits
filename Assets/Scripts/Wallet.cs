using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;

    [SerializeField] private double _moneyAmount;
    public double moneyAmount
    {
        get { return _moneyAmount; }
        set
        {
            _moneyAmount = value;
            RefreshUI();
        }
    }

    public List<double> bets = new List<double>()
    { 0.2, 0.4, 0.6, 0.8, 1, 1.2, 1.4, 1.6, 1.8, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 18, 20, 24, 30, 36, 40, 42, 48, 50, 54, 60, 70, 80, 90,
        100, 120, 140, 160, 180, 200, 240, 280, 300, 320, 360, 400, 500, 600, 700, 800, 900, 1000 };

    [HideInInspector] public double bet;
    public int betIndex;
    public string currency = "$";

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
        bet = bets[betIndex];
        RefreshUI();
    }

    public bool TryRemoveMoney(double money)
    {
        if (moneyAmount < money)
            return false;

        moneyAmount -= money;
        return true;
    }

    public void IncreaseBet()
    {
        if (betIndex < bets.Count - 1)
            betIndex++;
        bet = bets[betIndex];
        RefreshUI();
    }

    public void DecreaseBet()
    {
        if (betIndex > 0)
            betIndex--;
        bet = bets[betIndex];
        RefreshUI();
    }

    void RefreshUI()
    {
        UIManager.Instance.SetCreditText(currency + moneyAmount.ToString("#,0.00"));
        UIManager.Instance.SetBetText(currency + bet.ToString("#,0.00"));
    }
}
