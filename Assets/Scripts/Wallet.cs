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

    public double bet;
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
        RefreshUI();
    }

    public bool TryRemoveMoney(double money)
    {
        if (moneyAmount < money)
            return false;

        moneyAmount -= money;
        return true;
    }

    void RefreshUI()
    {
        UIManager.Instance.SetCreditText(moneyAmount.ToString("0,0.00") + " " + currency);
        UIManager.Instance.SetBetText(bet.ToString("0,0.00") + " " + currency);
    }
}
