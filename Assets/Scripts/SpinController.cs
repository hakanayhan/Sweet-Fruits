using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    public static SpinController Instance;

    public float normalGravityScale;
    public float turboGravityScale;
    public float fastGravityScale;

    private float _gravityScale;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void SetSpinSpeed(int i)
    {
        _gravityScale = (i == 0) ? turboGravityScale : (i == 1) ? fastGravityScale : normalGravityScale;

        foreach (GameObject fruit in GameController.Instance.fruits)
        {
            fruit.GetComponent<Rigidbody2D>().gravityScale = _gravityScale;
        }
    }
}
