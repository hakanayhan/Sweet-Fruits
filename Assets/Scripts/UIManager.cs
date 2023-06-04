using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI creditText;
    public TextMeshProUGUI betText;
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
}
