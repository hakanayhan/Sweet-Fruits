using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsController : MonoBehaviour
{
    public static FruitsController Instance;
    public List<GameObject> fruits = new List<GameObject>();
    public List<FruitSettings> fruitSettings = new List<FruitSettings>();
    public List<Transform> lines = new List<Transform>();
    public GameObject prefab;
    public List<int> spawnOrder = new List<int>();
    public int fruitAmount = 0;
    int maxFruitAmount = 30;
    int i = 0;
    float nextSpawnTime;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Update()
    {
        if (fruitAmount < maxFruitAmount)
        {
            spawnOrder.Add(i);
            i++;
            fruitAmount++;
            if (i > 5)
                i = 0;
        }

        if (spawnOrder.Count > 0 && Time.time > nextSpawnTime)
        {
            SpawnNewFruit(spawnOrder[0]);
            spawnOrder.RemoveAt(0);
            SetDelayTime();
        }
    }

    public void SpawnNewFruit(int a)
    {
        GameObject g = Instantiate(prefab, lines[a]);
        fruits.Add(g);
        g.GetComponent<FruitController>().currentLine = a;
        int r = Random.Range(0, fruitSettings.Count);
        g.GetComponent<FruitController>().SetFruitSettings(fruitSettings[r]);
    }

    void SetDelayTime()
    {
        nextSpawnTime = Time.time + 0.07f;
    }
}