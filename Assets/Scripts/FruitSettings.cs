using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Fruit Settings")]
public class FruitSettings : ScriptableObject
{
    public Sprite sprite;
    public float spawnRate;
    public float bonusSpawnRate;
    public bool bonus;
    public double eightNinePaymentMultiplier;
    public double tenElevenPaymentMultiplier;
    public double twelveAndAbovePaymentMultiplier;
}
