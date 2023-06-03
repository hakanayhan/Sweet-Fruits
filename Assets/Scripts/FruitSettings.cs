using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Fruit Settings")]
public class FruitSettings : ScriptableObject
{
    public Sprite sprite;
    public float spawnRate;
}
