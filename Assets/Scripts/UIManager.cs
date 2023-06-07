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
        FruitsController.Instance.StartBonusGame();
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
}
