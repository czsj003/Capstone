using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReplayGame(string sceneName)
    {
        SceneManager.LoadScene("__Scene_L1");
    }
}
