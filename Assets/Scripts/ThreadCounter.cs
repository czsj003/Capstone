using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThreadCounter : MonoBehaviour
{
    private TextMeshProUGUI textObj;
    // Start is called before the first frame update
    void Awake()
    {
        this.textObj = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        this.textObj.text = "Threads: " + Player.threads;
    }
}
