using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeed : MonoBehaviour
{
    [SerializeField]
    public int seed;
    public static RandomSeed S;

    private void Awake()
    {
        Random.InitState(seed);
        Debug.Log(seed);
        S = this;
    }
}
