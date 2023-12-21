using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance;
    public List<int> spawnOrder = new List<int>();
    public GameObject prefab;

    [SerializeField] private GameObject upperObject;
    bool destroyUpper;
    bool destroyUpperActivated;

    public List<int> spawnOrderCount = new List<int>() { 0, 0, 0, 0, 0, 0 };

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
        if (destroyUpper)
        {
            if (upperObject.transform.position.x >= 18)
            {
                destroyUpper = false;
                upperObject.SetActive(false);
                upperObject.transform.position -= new Vector3(15, 0, 0);
            }
            else
            {
                upperObject.transform.position += new Vector3(1, 0, 0);
            }
        }

        if (GameController.Instance.readyToSpawn && spawnOrder.Count > 0)
        {
            upperObject.SetActive(true);
            destroyUpperActivated = false;
            SpawnOrders();
        }
        else
        {
            if (!destroyUpperActivated)
            {
                destroyUpperActivated = true;
                if (GameController.Instance.isGameStarted)
                {
                    destroyUpper = false;
                    upperObject.SetActive(false);
                }
                else
                {
                    destroyUpper = true;
                }
                ResetSpawnOrderCount();
            }
        }
    }

    void SpawnOrders()
    {
        for (int i = 0; i < spawnOrder.Count; i++)
        {
            int order = spawnOrder[i];
            SpawnNewFruit(order);
            spawnOrder.RemoveAt(i);
        }
    }

    public void SpawnNewFruit(int a)
    {
        FruitSettings settings = SessionController.Instance.columns[a].spawnOrder[0];
        GameObject g = SessionController.Instance.columns[a].availableFruits[0];
        SessionController.Instance.columns[a].availableFruits.RemoveAt(0);
        g.GetComponent<FruitController>().currentColumn = a;
        g.transform.position = SessionController.Instance.columns[a].columnTransform.position;
        g.transform.position += new Vector3(-1f, -2.3f + (2 * spawnOrderCount[a]), 0f);
        g.GetComponent<FruitController>().SetFruitSettings(settings);
        g.SetActive(true);
        GameController.Instance.fruits.Add(g);
        SessionController.Instance.columns[a].spawnOrder.RemoveAt(0);

        spawnOrderCount[a] += 1;

        if (settings.name == SessionController.Instance.fruitSettings[10].name)
            g.GetComponent<FruitController>().SetMultiplier();
    }

    public void CreateSpawnOrder(int column)
    {
        spawnOrder.Add(column);
    }

    void ResetSpawnOrderCount()
    {
        for(int i = 0; i < 6; i++)
            spawnOrderCount[i] = 0;
    }
}
