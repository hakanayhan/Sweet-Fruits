using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoplayController : MonoBehaviour
{
    public static AutoplayController Instance;
    public bool autoSpin;
    public int autoSpinAmount;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void ActivateAutoSpin(int amount)
    {
        autoSpin = true;
        autoSpinAmount = amount;
        if (!GameController.Instance.onGame && !SessionController.Instance.bonusGame && !SessionController.Instance.activateBonusGame)
        {
            autoSpinAmount--;
            UIManager.Instance.autoplayText.text = autoSpinAmount + " LEFT";
            GameController.Instance.Spin();
        }
    }

    public void CheckAutoSpin()
    {
        if (autoSpin && autoSpinAmount == 0)
        {
            AutoplayWindow.Instance.OpenWindow();
        }

        if (autoSpin)
        {
            autoSpinAmount--;
            UIManager.Instance.autoplayText.text = autoSpinAmount + " LEFT";
            GameController.Instance.Spin();
        }
    }
}
