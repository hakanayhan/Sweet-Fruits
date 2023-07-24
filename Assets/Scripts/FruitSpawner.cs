using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance;
    public List<int> spawnOrder = new List<int>();
    public GameObject prefab;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.readyToSpawn && spawnOrder.Count > 0)
        {
            SpawnOrders();
        }
    }

    void SpawnOrders()
    {
        for (int i = 0; i < spawnOrder.Count; i++)
        {
            int order = spawnOrder[i];
            if (IsFruitNotInSpawnArea(order))
            {
                SpawnNewFruit(order);
                spawnOrder.RemoveAt(i);
            }
        }
    }

    bool IsFruitNotInSpawnArea(int i)
    {
        foreach (var fruit in GameController.Instance.fruits)
        {
            if (fruit.GetComponent<FruitController>().currentColumn == i && fruit.transform.position.y > SessionController.Instance.columns[0].columnTransform.position.y - 2.31)
            {
                return false;
            }
        }
        return true;
    }

    public void SpawnNewFruit(int a)
    {
        FruitSettings settings = SessionController.Instance.columns[a].spawnOrder[0];
        GameObject g = SessionController.Instance.columns[a].availableFruits[0];
        SessionController.Instance.columns[a].availableFruits.RemoveAt(0);
        g.GetComponent<FruitController>().currentColumn = a;
        g.transform.position = SessionController.Instance.columns[a].columnTransform.position;
        g.transform.position += new Vector3(-1f, -2.3f, 0f);
        g.GetComponent<FruitController>().SetFruitSettings(settings);
        g.SetActive(true);
        GameController.Instance.fruits.Add(g);
        SessionController.Instance.columns[a].spawnOrder.RemoveAt(0);

        if (settings.name == SessionController.Instance.fruitSettings[10].name)
            g.GetComponent<FruitController>().SetMultiplier();
    }

    public void CreateSpawnOrder(int column)
    {
        spawnOrder.Add(column);
    }
}
