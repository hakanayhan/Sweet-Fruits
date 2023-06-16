using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] TextMeshProUGUI creditText;
    [SerializeField] TextMeshProUGUI betText;
    [SerializeField] TextMeshProUGUI sessionWinText;

    [SerializeField] GameObject sessionWinGameObject;
    [SerializeField] GameObject spinToWinGameObject;
    [SerializeField] GameObject goodLuckGameObject;
    [SerializeField] GameObject bonusLeftGameObject;

    public TextMeshProUGUI autoplayText;
    [SerializeField] TextMeshProUGUI bonusLeftText;

    public GameObject tumbleGameObject;
    [SerializeField] TextMeshProUGUI tumbleText;

    [SerializeField] GameObject bonusPopup;

    [SerializeField] TextMeshProUGUI bonusBuyCostText;
    [SerializeField] TextMeshProUGUI bonusBuyCostWindowText;
    [SerializeField] GameObject bonusBuyWindow;

    [SerializeField] GameObject paymentScreen;
    [SerializeField] TextMeshProUGUI paymentScreenText;
    [SerializeField] TextMeshProUGUI paymentScreenWinText;

    [SerializeField] Button bonusBuyButton;
    [SerializeField] Button doubleChanceButton;
    [SerializeField] Button decreaseBetButton;
    [SerializeField] Button increaseBetButton;

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
        autoplayText.text = "AUTOPLAY";
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

    public void RefreshButtons()
    {
        if (GameController.Instance.onGame || SessionController.Instance.bonusGame)
        {
            bonusBuyButton.interactable = false;
            doubleChanceButton.interactable = false;
            increaseBetButton.interactable = false;
            decreaseBetButton.interactable = false;
        }
        else
        {
            bonusBuyButton.interactable = (SessionController.Instance.doubleChance) ? false : true;
            doubleChanceButton.interactable = true;
            increaseBetButton.interactable = true;
            decreaseBetButton.interactable = true;
        }
        
    }
}
