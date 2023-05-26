using System.Collections.Generic;
using UnityEngine;

public class SpriteLibrary : MonoBehaviour
{
    private Dictionary<Sprite, int> spriteIndex;
    private Sprite[] spriteList;

    void LoadAllSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Tiles/");
        print($"found {sprites.Length} sprites");
        spriteIndex = new Dictionary<Sprite, int>();
        spriteList = new Sprite[sprites.Length];
        
        int idx = 0;
        foreach (Sprite sprite in sprites)
        {
            spriteIndex[sprite] = idx;
            spriteList[idx] = sprite;
            idx++;
        }
    }

    public int GetSpriteId(Sprite sprite)
    {
        if (spriteIndex == null)
            LoadAllSprites();
        return spriteIndex[sprite];
    }
    
    public Sprite GetSpriteById(int id)
    {
        if (spriteList == null)
            LoadAllSprites();
        return spriteList[id];
    }
}
