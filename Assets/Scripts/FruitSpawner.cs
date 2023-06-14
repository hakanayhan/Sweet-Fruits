using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance;
    public List<int> spawnOrder = new List<int>();
    public GameObject prefab;
    private float _spawnOffset = -2f;
    private float _nextSpawnTime;
    [SerializeField] private float _spawnDelay;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (GameController.Instance.readyToSpawn && spawnOrder.Count > 0 && Time.time > _nextSpawnTime)
        {
            SpawnNewFruit(spawnOrder[0]);
            spawnOrder.RemoveAt(0);
            SetDelayTime();
            _spawnOffset += 0.5f;
        }
        if (spawnOrder.Count == 0 && GameController.Instance.IsEveryFruitOnReel())
            _spawnOffset = -2f;
    }

    public void SpawnNewFruit(int a)
    {
        FruitSettings settings = SessionController.Instance.columns[a].spawnOrder[0];
        GameObject g = SessionController.Instance.columns[a].availableFruits[0];
        SessionController.Instance.columns[a].availableFruits.RemoveAt(0);
        g.transform.position = SessionController.Instance.columns[a].columnTransform.position;
        g.transform.position += new Vector3(-1f, _spawnOffset, 0f);
        g.GetComponent<FruitController>().SetFruitSettings(settings);
        g.SetActive(true);
        GameController.Instance.fruits.Add(g);
        SessionController.Instance.columns[a].spawnOrder.RemoveAt(0);

        if (settings.name == SessionController.Instance.fruitSettings[10].name)
            g.GetComponent<FruitController>().SetMultiplier();
    }

    void SetDelayTime()
    {
        _nextSpawnTime = Time.time + _spawnDelay;
    }

    public void CreateSpawnOrder(int column)
    {
        spawnOrder.Add(column);
    }
}
