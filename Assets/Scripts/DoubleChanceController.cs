using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoubleChanceController : MonoBehaviour
{
    public static DoubleChanceController Instance;
    public float doubleChanceEffect = 1;
    [SerializeField] TextMeshProUGUI doubleChanceCostText;
    [SerializeField] GameObject onImage;
    [SerializeField] GameObject offImage;

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
        SetDoubleChanceCostText();
    }

    public void DoubleChanceButton()
    {
        if(!SessionController.Instance.bonusGame && !GameController.Instance.onGame && !AutoplayController.Instance.autoSpin)
        {
            if (SessionController.Instance.doubleChance)
            {
                offImage.SetActive(true);
                onImage.SetActive(false);
                SessionController.Instance.doubleChance = false;
            }
            else
            {
                onImage.SetActive(true);
                offImage.SetActive(false);
                SessionController.Instance.doubleChance = true;
            }
            Wallet.Instance.RefreshUI();
        }
    }

    public void SetDoubleChanceCostText()
    {
        doubleChanceCostText.text = Wallet.Instance.currency + (Wallet.Instance.bet * 1.25).ToString("#,0.00");
    }
}
