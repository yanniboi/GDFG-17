using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelStorage
{
    public static string LevelStoragePath => Application.persistentDataPath + "/levels/";
    public static string LevelStorageCurrentLevel => Application.persistentDataPath + "/levels/level.json";

    public string LevelCode;

    public static void CheckStoragePath()
    {
        var directoryInfo = new FileInfo(LevelStorageCurrentLevel).Directory;
        if (directoryInfo != null)
        {
            directoryInfo.Create();
        }
    }
}

[Serializable]
public class LevelTiles
{
    public List<Vector3> tiles = new List<Vector3>();
}
