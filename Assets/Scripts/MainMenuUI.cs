using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene("__Scene_L1");
    }

    public void OpenAudioCredits() { creditsPanel.SetActive(true); }
    public void CloseAudioCredits() { creditsPanel.SetActive(false); }
}
