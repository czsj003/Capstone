using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerationUpLeft : GenerateRoom
{

    protected override void generateRoom() //TODO. Pick random room from roomGenerationOptions (based on seed), spawn it next to current room.
    {
        int roomToGen = UnityEngine.Random.Range(0, roomGenerationOptions.Length); //Currently untested, need more rooms.
        float newX = (float)(Math.Ceiling(this.gameObject.transform.position.x) - 11);
        float newY = (float)(Math.Ceiling(this.gameObject.transform.position.y)); //Assumes all rooms are of uniform size!!!
        Instantiate(roomGenerationOptions[roomToGen], new Vector3(newX, newY, 0), this.transform.rotation);
        Debug.Log($"Generating room index: {roomToGen} of {roomGenerationOptions.Length}");
    }

    protected override void generateBossRoom() //TODO. Pick random room from roomGenerationOptions (based on seed), spawn it next to current room.
    {
        int roomToGen = UnityEngine.Random.Range(0, bossRoomOptions.Length); //Currently untested, need more rooms.
        float newX = (float)(Math.Ceiling(this.gameObject.transform.position.x) - 11);
        float newY = (float)(Math.Ceiling(this.gameObject.transform.position.y)); //Assumes all rooms are of uniform size!!!
        Instantiate(bossRoomOptions[roomToGen], new Vector3(newX, newY, 0), this.transform.rotation);
        Debug.Log($"Generating room index: {roomToGen} of {bossRoomOptions.Length}");
    }
}
