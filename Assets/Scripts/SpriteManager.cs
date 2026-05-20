using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager
{
    static private Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();

    static public void AddSprite(string key, Sprite value) { spriteMap.Add(key, value); }
    static public Sprite GetSprite(string key) {  return spriteMap[key]; }
}
