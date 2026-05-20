using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformMover : MonoBehaviour
{
    public float speed = 1.0f;  // 升降速度
    private float maxHeight;     // 最大高度
    private float minHeight;     // 最小高度
    private float direction = 1.0f; // 方向，1表示向上，-1表示向下

    void Start()
    {
        // 获取当前高度
        float currentHeight = transform.position.y;
        minHeight = currentHeight;        // 设置最小高度为当前高度
        maxHeight = currentHeight + 4.0f; // 设置最大高度为当前高度 + 4
    }

    void Update()
    {
        // 按时间移动Cube
        transform.position += Vector3.up * direction * speed * Time.deltaTime;

        // 检查是否达到最大或最小高度
        if (transform.position.y >= maxHeight)
        {
            direction = -1.0f; // 到达最大高度，改变方向
        }
        else if (transform.position.y <= minHeight)
        {
            direction = 1.0f; // 到达最小高度，改变方向
        }
    }
}
