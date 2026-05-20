using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateRoom : MonoBehaviour //Attach to door object in a room prefab
{
    [SerializeField]
    public GameObject[] roomGenerationOptions; //Modify in editor, list of all potential rooms.
    public GameObject[] bossRoomOptions;

    private bool touched = false;
    private float timer = 0.0f;
    private float disDuration = 0.25f;
    private float yVal = 2.0f;
    private GameObject loadedEnemies;

    private int lvl1Count = 5;

    protected void Awake()
    {
        this.loadedEnemies = GameObject.Find("loadedEnemies");
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (loadedEnemies.transform.childCount == 0)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (!touched)
                {
                    Player player = FindObjectOfType<Player>();
                    if (player != null)
                    {
                        player.roomPassed += 1;
                        Debug.Log(player.roomPassed);

                        if (player.roomPassed == lvl1Count)
                        {
                            SceneManager.LoadScene("__Scene_L2");
                        }
                        else if (player.roomPassed == lvl1Count * 2)
                        {
                            SceneManager.LoadScene("__Scene_L3");
                        }
                    }
                    else
                    {
                        Debug.LogError("can't find the Player");
                    }
                    if (player.roomPassed > 0 && player.roomPassed % lvl1Count == lvl1Count-1)
                    {
                        generateBossRoom();
                    }
                    else if (player.roomPassed > 0 && player.roomPassed % lvl1Count != 0)
                    {
                        generateRoom();
                    }
                    touched = true;
                }
            }
        }
    }

    protected void Update()
    {
        if(touched)
        {
            if (timer < disDuration)
            {
                this.gameObject.transform.localScale = new Vector3(0.5f, yVal * (1 - (timer / disDuration)), 1);
                timer += Time.deltaTime;
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void generateRoom() //TODO. Pick random room from roomGenerationOptions (based on seed), spawn it next to current room.
    {
        int roomToGen = UnityEngine.Random.Range(0, roomGenerationOptions.Length); //Currently untested, need more rooms.
        float newX = (float)(Math.Ceiling(this.gameObject.transform.position.x) + 10); //Assumes all rooms are of uniform size!!!
        float newY = (float)(Math.Ceiling(this.gameObject.transform.position.y));
        Instantiate(roomGenerationOptions[roomToGen], new Vector3(newX, newY, 0), this.transform.rotation);
        Debug.Log($"Generating room index: {roomToGen} of {roomGenerationOptions.Length}");
    }
    protected virtual void generateBossRoom() //TODO. Pick random room from roomGenerationOptions (based on seed), spawn it next to current room.
    {
        int roomToGen = UnityEngine.Random.Range(0, bossRoomOptions.Length); //Currently untested, need more rooms.
        float newX = (float)(Math.Ceiling(this.gameObject.transform.position.x) + 10); //Assumes all rooms are of uniform size!!!
        float newY = (float)(Math.Ceiling(this.gameObject.transform.position.y));
        Instantiate(bossRoomOptions[roomToGen], new Vector3(newX, newY, 0), this.transform.rotation);
        Debug.Log($"Generating room index: {roomToGen} of {bossRoomOptions.Length}");
    }
}
