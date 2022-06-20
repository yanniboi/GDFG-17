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
public class LevelData
{
    public List<LevelChunk> chunks = new List<LevelChunk>();
    public List<LevelChunk> Chunks
    {
        get { return chunks; }
        set { chunks = value; }
    }
}

[Serializable]
public class LevelChunk
{
    public List<Vector3> tiles = new List<Vector3>();
    public List<Vector3> Tiles
    {
        get { return tiles; }
        set { tiles = value; }
    }


    public LevelChunk(List<Vector3> tiles)
    {
        this.Tiles = tiles;
    }
    public LevelChunk()
    {

    }

}
