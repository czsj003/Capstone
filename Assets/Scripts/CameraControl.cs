using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Transform player;

    int movingHorizontal = 0;
    int movingVertical = 0;
    Vector3 startingPos;

    float tHorizontal = 0;
    float tVertical = 0;
    float duration = 0.75f;

    float verticalThreshold = 3.0f;
    float horizontalThreshold = 10f;

    void Awake()
    {
        updateStartPos();
    }

    void Update()
    {
        // Get the player's current position
        float playerHeight = player.position.y;

        // Check for vertical position changes
        if (player.position.y > startingPos.y + verticalThreshold && movingVertical == 0)
        {
            movingVertical = 1;
            tVertical = 0;
        }
        if (player.position.y < startingPos.y - verticalThreshold && movingVertical == 0)
        {
            movingVertical = -1;
            tVertical = 0;
        }

        // Check for horizontal position changes
        if (player.position.x > startingPos.x + horizontalThreshold && movingHorizontal == 0)
        {
            movingHorizontal = 1;
            tHorizontal = 0;
        }
        if (player.position.x < startingPos.x - horizontalThreshold && movingHorizontal == 0)
        {
            movingHorizontal = -1;
            tHorizontal = 0;
        }

        // Smoothly update the camera's horizontal position
        if (movingHorizontal != 0)
        {
            this.gameObject.transform.position = Vector3.Lerp(
                startingPos,
                new Vector3(startingPos.x + (20 * movingHorizontal), this.gameObject.transform.position.y, -10),
                tHorizontal / duration
            );
            tHorizontal += Time.deltaTime;
        }

        // Smoothly update the camera's vertical position
        if (movingVertical != 0)
        {
            this.gameObject.transform.position = Vector3.Lerp(
                startingPos,
                new Vector3(this.gameObject.transform.position.x, startingPos.y + (6 * movingVertical), -10),
                tVertical / duration
            );
            tVertical += Time.deltaTime;
        }

        // End of horizontal movement
        if (tHorizontal >= duration && movingHorizontal != 0)
        {
            this.gameObject.transform.position = new Vector3(startingPos.x + (20 * movingHorizontal), this.gameObject.transform.position.y, -10);
            updateStartPos();
            movingHorizontal = 0;
        }

        // End of vertical movement
        if (tVertical >= duration && movingVertical != 0)
        {
            this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, startingPos.y + (6 * movingVertical), -10);
            updateStartPos();
            movingVertical = 0;
        }
    }

    void updateStartPos()
    {
        this.startingPos = this.gameObject.transform.position;
    }
}
