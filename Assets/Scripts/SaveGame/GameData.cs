using UnityEngine;
using System;

[Serializable]
public class BuildingData
{
    public Vector3 position;
    public int prefabId;
    public string tag;
}

[Serializable]
public class GameData
{
    [Header("Player Progress")]
    public int money;
    public int lvlPlayer;

    [Header("Animals Status")]
    public bool isCowUnlocked;
    public bool isLamaUnlocked;

    [Header("Buildings")]
    public BuildingData[] buildings;

    // Конструктор для создания новой игры
    public static GameData CreateNewGame()
    {
        return new GameData
        {
            money = 100,
            lvlPlayer = 1,
            isCowUnlocked = false,
            isLamaUnlocked = false,
            buildings = new BuildingData[0]
        };
    }
}