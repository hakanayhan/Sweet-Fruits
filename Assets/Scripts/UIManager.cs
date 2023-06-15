using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI creditText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI sessionWinText;

    public GameObject sessionWinGameObject;
    public GameObject spinToWinGameObject;
    public GameObject goodLuckGameObject;
    public GameObject bonusLeftGameObject;

    public TextMeshProUGUI autoplayText;
    public TextMeshProUGUI bonusLeftText;

    public GameObject tumbleGameObject;
    public TextMeshProUGUI tumbleText;

    public GameObject bonusPopup;

    public TextMeshProUGUI bonusBuyCostText;
    public TextMeshProUGUI bonusBuyCostWindowText;
    public GameObject bonusBuyWindow;

    public GameObject paymentScreen;
    public TextMeshProUGUI paymentScreenText;
    public TextMeshProUGUI paymentScreenWinText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OpenBonusPopup()
    {
        bonusPopup.SetActive(true);
    }

    public void StartBonus()
    {
        bonusPopup.SetActive(false);
        SessionController.Instance.StartBonusGame();
    }

    public void BuyBonusFeature()
    {
        bonusBuyWindow.SetActive(false);
        GameController.Instance.BuySpin();
    }

    public void SetCreditText(string text)
    {
        creditText.text = text;
    }

    public void SetBetText(string text)
    {
        betText.text = text;
    }
    public void SetBonusLeftText(string text)
    {
        bonusLeftGameObject.SetActive(true);
        bonusLeftText.text = text;
    }

    public void CloseBonusLeftText()
    {
        bonusLeftGameObject.SetActive(false);
    }

    public void SetSessionWinText(string text)
    {
        goodLuckGameObject.SetActive(false);
        spinToWinGameObject.SetActive(false);
        sessionWinGameObject.SetActive(true);
        sessionWinText.text = text;
    }

    public void ActivateSpinToWinText()
    {
        sessionWinGameObject.SetActive(false);
        goodLuckGameObject.SetActive(false);
        spinToWinGameObject.SetActive(true);
    }

    public void ActivateGoodLuckText()
    {
        sessionWinGameObject.SetActive(false);
        spinToWinGameObject.SetActive(false);
        goodLuckGameObject.SetActive(true);
    }

    public void SetTumbleText(string text)
    {
        tumbleGameObject.SetActive(true);
        tumbleText.text = text;
    }
    public void OpenBonusBuyWindow()
    {
        if(Wallet.Instance.moneyAmount >= Wallet.Instance.GetBonusBuyCost() && !GameController.Instance.onGame && !AutoplayController.Instance.autoSpin && !SessionController.Instance.doubleChance)
        {
            bonusBuyCostWindowText.text = Wallet.Instance.currency + Wallet.Instance.GetBonusBuyCost().ToString("#,0");
            bonusBuyWindow.SetActive(true);
        }
    }

    public void CloseBonusBuyWindow()
    {
        bonusBuyWindow.SetActive(false);
    }

    public void SetBonusBuyCostText(string text)
    {
        bonusBuyCostText.text = text;
    }

    public void OpenPaymentScreen(string screenText, string winText)
    {
        paymentScreenText.text = screenText;
        paymentScreenWinText.text = winText;
        paymentScreen.SetActive(true);
    }
    public void ClosePaymentScreen()
    {
        paymentScreen.SetActive(false);
        if (SessionController.Instance.bonusGame)
        {
            SessionController.Instance.EndBonusSession();
        }
        else
        {
            SessionController.Instance.EndSession();
        }
    }
}
