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


    [SerializeField] private int width;
    public int Width { get { return width; } set { width = value; } }

    [SerializeField] private int height;
    public int Height { get { return height; } set { height = value; } }

    [SerializeField] private int areaTilesX;
    public int AreaTilesX { get { return areaTilesX; } set { areaTilesX = value; } }

    [SerializeField] private int areaTilesY;
    public int AreaTilesY { get { return areaTilesY; } set { areaTilesY = value; } }


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


    public int[] states;
    public int[] States
    {
        get { return states; }
        set { states = value; }
    }



    public LevelChunk(List<Vector3> tiles)
    {
        this.Tiles = tiles;
    }
    public LevelChunk()
    {

    }

}
