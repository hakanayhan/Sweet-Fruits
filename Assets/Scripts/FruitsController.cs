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
            bottomObject.SetActive(false);
            onSpin = true;
            onGame = true;
        }
    }

    public void SpawnNewFruit(int a)
    {
        GameObject g = Instantiate(prefab, lines[a]);
        fruits.Add(g);
        g.GetComponent<FruitController>().currentLine = a;
        g.GetComponent<FruitController>().SetFruitSettings(GetRandomFruitBySpawnRate());
    }

    void SetDelayTime()
    {
        nextSpawnTime = Time.time + 0.07f;
    }

    private FruitSettings GetRandomFruitBySpawnRate()
    {

        float randomValue = Random.Range(0, totalSpawnRate);
        float cumulativeSpawnRate = 0;

        foreach (FruitSettings fruitSetting in fruitSettings)
        {
            cumulativeSpawnRate += fruitSetting.spawnRate;
            if (randomValue < cumulativeSpawnRate)
            {
                return fruitSetting;
            }
        }

        // Eðer bir eþleþme bulunamazsa, varsayýlan olarak ilk meyve ayarýný döndür
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
            spawnOrder.Add(fruit.GetComponent<FruitController>().currentLine);
            Destroy(fruit);
            fruits.Remove(fruit);
        }
    }
}