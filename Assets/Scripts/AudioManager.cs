using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource levelBGMStart, levelBGMLoop;

    // Start is called before the first frame update
    void Start()
    {
        levelBGMStart = GetComponent<AudioSource>();
        levelBGMLoop = GetComponents<AudioSource>()[1];

        levelBGMStart.Play();
        levelBGMLoop.PlayDelayed(levelBGMStart.clip.length - 0.01f);
    }

    static public void PLAY_DEATH_SOUND(AudioClip clip, Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(clip, pos);
    }
}
