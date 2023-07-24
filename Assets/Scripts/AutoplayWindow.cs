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
    [SerializeField] GameObject bg;
    [SerializeField] List<Toggle> toggles = new List<Toggle>();
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
    public void OpenWindowButton()
    {
        if(!SessionController.Instance.bonusGame && !AutoplayController.Instance.autoSpin)
        {
            bg.SetActive(!window.activeSelf);
            window.SetActive(!window.activeSelf);
        }
        if (AutoplayController.Instance.autoSpin)
        {
            AutoplayController.Instance.autoSpinAmount = 0;
            UIManager.Instance.autoplayText.text = "AUTOPLAY";
        }
    }
    public void CloseWindowButton()
    {
        CloseWindow();
    }

    public void OpenWindow()
    {
        bg.SetActive(true);
        window.SetActive(true);
    }

    public void CloseWindow()
    {
        bg.SetActive(false);
        window.SetActive(false);
    }

    public void SetAmount()
    {
        amount = (int)slider.value;
        amountText.text = amount.ToString();
        buttonText.text = "START AUTOPLAY (" + amount.ToString() + ")";
    }

    public void StartButton()
    {
        AutoplayController.Instance.ActivateAutoSpin(amount);
        CloseWindow();
    }

    public void CheckToggles(int i)
    {
        if (toggles[i].isOn)
        {
            foreach(Toggle toggle in toggles)
            {
                if(toggle != toggles[i])
                    toggle.isOn = false;
            }
            toggles[i].graphic.gameObject.SetActive(true);
            SpinController.Instance.SetSpinSpeed(i);
        }
        else
        {
            toggles[i].graphic.gameObject.SetActive(false);
            SpinController.Instance.SetSpinSpeed(-1);
        }
    }
}
