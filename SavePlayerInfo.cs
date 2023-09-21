using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script contains any necessary saved data about the player.
*/

public static class SavePlayerInfo
{
    public static int susLevel;
    public static Vector3 position;
    public static float rotationY;

    static SavePlayerInfo()
    {
        susLevel = 0;
        position = new Vector3(-12.89f, 7.3f, -3.68f);
        rotationY = 0;
    }
}
