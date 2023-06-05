using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoplayWindow : MonoBehaviour
{
    public static AutoplayWindow Instance;
    public int amount;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] GameObject window;
    [SerializeField] Slider slider;
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
        SetAmount();
    }
    public void OpenWindow()
    {
        window.SetActive(!window.activeSelf);
        if (FruitsController.Instance.autoSpin)
        {
            FruitsController.Instance.autoSpin = false;
            UIManager.Instance.autoplayText.text = "AUTOPLAY";
        }
    }
    public void CloseWindow()
    {
        window.SetActive(!window.activeSelf);
    }

    public void SetAmount()
    {
        amount = (int)slider.value;
        amountText.text = amount.ToString();
        buttonText.text = "START AUTOPLAY (" + amount.ToString() + ")";
    }

    public void StartButton()
    {
        FruitsController.Instance.ActivateAutoSpin(amount);
        CloseWindow();
    }
}
