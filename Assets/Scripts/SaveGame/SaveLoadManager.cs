using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class SaveLoadManager : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private float autoSaveInterval = 30f;
    private float autoSaveTimer = 0f;
    
    private string filePath;
    private GameData currentGameData;

    private void Awake()
    {
        InitializeSaveSystem();
    }

    private void InitializeSaveSystem()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        currentGameData = LoadGame() ?? new GameData();
    }

    private void Update()
    {
        HandleAutoSave();
    }

    private void HandleAutoSave()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            SaveGame(currentGameData);
            autoSaveTimer = 0f;
            Debug.Log("Автосохранение выполнено");
        }
    }

    public void SaveGame(GameData gameData)
    {
        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Игра сохранена в: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при сохранении: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                GameData loadedData = new GameData();
                JsonUtility.FromJsonOverwrite(json, loadedData);
                Debug.Log("Данные загружены успешно");
                return loadedData;
            }
            Debug.Log("Файл сохранения не найден, создаются новые данные");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при загрузке: {e.Message}");
            return null;
        }
    }

    public void DeleteSaveData()
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Данные сохранения удалены");
            }
            else
            {
                Debug.LogWarning("Нет данных для удаления");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при удалении сохранения: {e.Message}");
        }
    }
}
