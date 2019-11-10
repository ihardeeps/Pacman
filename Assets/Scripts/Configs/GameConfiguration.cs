using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfiguration : ScriptableObject
{
    public static string AssetPath = "Configs/Configuration";
    //This is here for now, this need not be here. But this can be converted to anim curve so over playerlevel the grid size can change.
    public int GridSizeX, GridSizeY;
    public Sprite empty, blocked, closed_door, open_door, start;

    public void Init()
    {
        GridSizeX = GridSizeY = 0;
    }
}
