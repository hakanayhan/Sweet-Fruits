using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField] FruitSettings fruitSettings;
    public int currentLine;
    private void Update()
    {
        if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }
    public void SetFruitSettings(FruitSettings fruitSettings)
    {
        this.fruitSettings = fruitSettings;
        SetSprite();
    }

    void SetSprite()
    {
        Sprite fruitSprite = fruitSettings.sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = fruitSprite;
    }

    private void OnDestroy()
    {
        FruitsController.Instance.fruits.Remove(gameObject);
        FruitsController.Instance.spawnOrder.Add(currentLine);
    }
}
