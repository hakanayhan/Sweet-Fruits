using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsController : MonoBehaviour
{
    public static FruitsController Instance;
    public List<GameObject> fruits = new List<GameObject>();
    public List<FruitSettings> fruitSettings = new List<FruitSettings>();
    public List<Lines> lines;
    public GameObject prefab;
    public List<int> spawnOrder = new List<int>();
    public int fruitAmount = 0;
    int maxFruitAmount = 30;
    int i = 0;
    float nextSpawnTime;

    public bool onSpin = false;
    public bool onGame = true;
    [SerializeField] GameObject bottomObject;

    float totalSpawnRate = 0;

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
        CalculateTotalSpawnRate();
        GenerateSpawnOrders();
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

        if (onSpin && fruits.Count == 0)
        {
            fruitAmount = 0;
            onSpin = false;
            bottomObject.SetActive(true);
        }

        if (!AnyFruitMoving() && fruitAmount == maxFruitAmount)
        {
            CheckMatchingFruits();
        }
    }

    void GenerateSpawnOrders()
    {
        foreach(Lines line in lines)
        {
            line.spawnOrder.Clear();
            for(int a = 0; a < 6; a++)
            {
                if(a == 0 && UnityEngine.Random.Range(0, 2) == 1)
                {
                    GenerateSpawnOrder(line, 1);
                }
                else
                {
                    GenerateSpawnOrder(line);
                }
                
            }
        }
    }

    void GenerateSpawnOrder(Lines line, int howMany = 2)
    {
        FruitSettings r = GetRandomFruitBySpawnRate();
        if(r.bonus)
            howMany = 1;

        for (int a = 0; a < howMany; a++)
        {
            line.spawnOrder.Add(r);
        }
    }

    bool AnyFruitMoving()
    {
        foreach (var fruit in fruits)
        {
            Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
            if (rb != null && rb.velocity.magnitude > 0f)
            {
                return true;
            }
        }
        return false;
    }

    public void Spin()
    {
        if (!onSpin && !onGame && !AnyFruitMoving())
        {
            spawnOrder.Clear();
            GenerateSpawnOrders();
            bottomObject.SetActive(false);
            onSpin = true;
            onGame = true;
        }
    }

    public void SpawnNewFruit(int a)
    {
        GameObject g = Instantiate(prefab, lines[a].lineTransform);
        fruits.Add(g);
        g.GetComponent<FruitController>().currentLine = a;
        g.GetComponent<FruitController>().SetFruitSettings(lines[a].spawnOrder[0]);
        lines[a].spawnOrder.RemoveAt(0);
    }

    void SetDelayTime()
    {
        nextSpawnTime = Time.time + 0.07f;
    }

    private FruitSettings GetRandomFruitBySpawnRate()
    {

        float randomValue = UnityEngine.Random.Range(0, totalSpawnRate);
        float cumulativeSpawnRate = 0;

        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            cumulativeSpawnRate += fruitSetting.spawnRate;
            if (randomValue < cumulativeSpawnRate)
            {
                return fruitSetting;
            }
        }
        return fruitSettings[0];
    }

    private void CalculateTotalSpawnRate()
    {
        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            totalSpawnRate += fruitSetting.spawnRate;
        }
    }

    void CheckMatchingFruits()
    {
        Dictionary<FruitSettings, int> fruitCount = new Dictionary<FruitSettings, int>();

        foreach (GameObject fruit in fruits)
        {
            FruitController fruitController = fruit.GetComponent<FruitController>();
            if (fruitController != null)
            {
                FruitSettings fruitSetting = fruitController.GetFruitSettings();
                if (!fruitCount.ContainsKey(fruitSetting))
                {
                    fruitCount[fruitSetting] = 1;
                }
                else
                {
                    fruitCount[fruitSetting]++;
                }
            }
        }

        bool hasMatchingFruits = false;

        foreach (KeyValuePair<FruitSettings, int> pair in fruitCount)
        {
            if (pair.Value >= 8)
            {
                List<GameObject> matchingFruits = GetMatchingFruits(pair.Key);
                ExplodeMatchingFruits(matchingFruits);
                hasMatchingFruits = true;
            }
        }
        if (!hasMatchingFruits && fruits.Count == maxFruitAmount)
        {
            onGame = false;
        }
    }

    List<GameObject> GetMatchingFruits(FruitSettings fruitSetting)
    {
        List<GameObject> matchingFruits = new List<GameObject>();

        foreach (GameObject fruit in fruits)
        {
            FruitController fruitController = fruit.GetComponent<FruitController>();
            if (fruitController != null && fruitController.GetFruitSettings() == fruitSetting)
            {
                matchingFruits.Add(fruit);
            }
        }

        return matchingFruits;
    }

    void ExplodeMatchingFruits(List<GameObject> matchingFruits)
    {
        foreach (GameObject fruit in matchingFruits)
        {
            int currentLine = fruit.GetComponent<FruitController>().currentLine;
            spawnOrder.Add(currentLine);
            GenerateSpawnOrder(lines[currentLine]);
            Destroy(fruit);
            fruits.Remove(fruit);
        }
    }
}

[Serializable] public class Lines
{
    public Transform lineTransform;
    [HideInInspector] public List<FruitSettings> spawnOrder = new List<FruitSettings>();
}