using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FruitController : MonoBehaviour
{
    [SerializeField] FruitSettings fruitSettings;
    public int currentColumn;
    [SerializeField] private GameObject _multiplierText;
    [HideInInspector] public int multiplier;
    List<int> multiplierList;

    private void FixedUpdate()
    {
        if(gameObject.activeSelf && transform.position.y < -6)
        {
            Deactive();
        }
    }
    public void SetFruitSettings(FruitSettings fruitSettings)
    {
        this.fruitSettings = fruitSettings;
        SetSprite();
    }

    public FruitSettings GetFruitSettings()
    {
        return fruitSettings;
    }

    void SetSprite()
    {
        Sprite fruitSprite = fruitSettings.sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = fruitSprite;
    }

    public void SetMultiplier()
    {
        multiplier = GetRandomMultiplier();
        _multiplierText.GetComponent<TextMeshPro>().text = multiplier.ToString() + "x";
        _multiplierText.SetActive(true);
    }

    int GetRandomMultiplier()
    {
        multiplierList = GameController.Instance.multiplierList;
        int lastIndex = multiplierList.Count - 1;
        int chanceToReturn = 1;
        for (int i = 0; i <= lastIndex; i++)
        {
            chanceToReturn = (i == 2) ? 2 : (i == 4) ? 3 : (i == 8) ? 5 : (i == 10) ? 8 : chanceToReturn;

            if (UnityEngine.Random.Range(1, 11) <= chanceToReturn || i == lastIndex)
                return multiplierList[i];
        }
        return 2;
    }

    public void Deactive()
    {
        gameObject.SetActive(false);
        _multiplierText.SetActive(false);
        GameController.Instance.fruits.Remove(gameObject);
        SessionController.Instance.columns[currentColumn].availableFruits.Add(gameObject);
    }
}
