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

    public TextMeshProUGUI autoplayText;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCreditText(string text)
    {
        creditText.text = text;
    }

    public void SetBetText(string text)
    {
        betText.text = text;
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
}
