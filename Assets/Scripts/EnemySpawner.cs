using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    public Enemy[] enemyPool; //Modify in editor, list of all potential enemies to spawn.
    
    private GameObject loadedEnemies;

    private void Awake()
    {
        this.loadedEnemies = GameObject.Find("loadedEnemies");
    }

    void Start()
    {
        int roomToGen = UnityEngine.Random.Range(0, enemyPool.Length); //Currently untested, need more enemies.
        Instantiate(enemyPool[roomToGen], this.gameObject.transform.position, this.transform.rotation, loadedEnemies.transform); //Spawns enemy at EnemySpawner location under parent gameobject "loadedEnemies".
    }
}

