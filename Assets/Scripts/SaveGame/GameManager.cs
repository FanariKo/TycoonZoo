using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    private SaveLoadManager saveLoadManager;
    private GameController money;
    private GameData currentGameData;

    [Header("Save Settings")]
    private float saveTimer = 0f;
    private float saveInterval = 30f;

    [Header("Building References")]
    [SerializeField] private GameObject[] buildingPrefabs; // Массив всех возможных префабов построек

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        money = GetComponent<GameController>();
        
        currentGameData = saveLoadManager.LoadGame();
        
        if (currentGameData == null)
        {
            Debug.Log("Первый запуск игры - устанавливаем начальные значения");
            currentGameData = GameData.CreateNewGame();
            saveLoadManager.SaveGame(currentGameData);
        }
        else
        {
            Debug.Log("Загружены существующие данные игры");
        }
        
        ApplyLoadedData();
    }

    private void ApplyLoadedData()
    {
        // Загружаем базовые данные
        money.money = currentGameData.money;
        GameController.playerMoney = currentGameData.money;
        PlayerPrefs.SetInt("Money", money.money);
        
        GameController.lvlPlayer = currentGameData.lvlPlayer;
        PlayerPrefs.SetInt("LvlPlayer", currentGameData.lvlPlayer);
        
        // Применяем состояние животных
        ApplyAnimalStates();
        
        // Загружаем постройки
        LoadBuildings();
        
        Debug.Log($"Данные загружены: Деньги={money.money}, Уровень={currentGameData.lvlPlayer}");
    }

    private void ApplyAnimalStates()
    {
        if (money.cow != null)
        {
            money.cow.GetComponent<CheckLvlPlayer>().isOpenAnimal = currentGameData.isCowUnlocked;
            if (currentGameData.isCowUnlocked)
            {
                Renderer renderer = money.cow.GetComponent<Renderer>();
                renderer.sharedMaterial.color = money.openColor;
            }
        }
        
        if (money.lama != null)
        {
            money.lama.GetComponent<CheckLvlPlayer>().isOpenAnimal = currentGameData.isLamaUnlocked;
            if (currentGameData.isLamaUnlocked)
            {
                Renderer renderer = money.lama.GetComponent<Renderer>();
                renderer.sharedMaterial.color = money.openColor;
            }
        }
    }

    private void LoadBuildings()
    {
        if (currentGameData.buildings != null && currentGameData.buildings.Length > 0)
        {
            Debug.Log($"Начинаем загрузку {currentGameData.buildings.Length} построек");
            foreach (var buildingData in currentGameData.buildings)
            {
                GameObject prefab = GetPrefabById(buildingData.prefabId);
                if (prefab != null)
                {
                    // Находим свободное место для постройки
                    GameObject freePlaceBuild = FindFreePlaceBuildAtPosition(buildingData.position);
                    if (freePlaceBuild != null)
                    {
                        // Создаем здание и устанавливаем его как дочерний объект для места постройки
                        // Добавляем смещение по Y для правильного позиционирования
                        Vector3 spawnPosition = buildingData.position + new Vector3(0, 0.45f, 0);
                        GameObject building = Instantiate(prefab, spawnPosition, Quaternion.identity);
                        building.transform.SetParent(freePlaceBuild.transform);
                        freePlaceBuild.tag = buildingData.tag;
                        Debug.Log($"Загружена постройка: ID={buildingData.prefabId}, Position={spawnPosition}");
                    }
                    else
                    {
                        Debug.LogWarning($"Не найдено свободное место для постройки на позиции {buildingData.position}");
                    }
                }
                else
                {
                    Debug.LogError($"Не удалось найти префаб с ID {buildingData.prefabId}");
                }
            }
        }
        else
        {
            Debug.Log("Нет сохраненных построек для загрузки");
        }
    }

    private GameObject FindFreePlaceBuildAtPosition(Vector3 position)
    {
        // Ищем все места для построек
        GameObject[] placeBuilds = GameObject.FindGameObjectsWithTag("FreePlaceBuild");
        
        // Находим ближайшее к сохраненной позиции место
        GameObject nearestPlace = null;
        float minDistance = float.MaxValue;
        
        foreach (GameObject place in placeBuilds)
        {
            float distance = Vector3.Distance(place.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlace = place;
            }
        }
        
        return nearestPlace;
    }

    private GameObject GetPrefabById(int prefabId)
    {
        foreach (GameObject prefab in buildingPrefabs)
        {
            PrefabID prefabID = prefab.GetComponent<PrefabID>();
            if (prefabID != null && prefabID.id == prefabId)
            {
                return prefab;
            }
        }
        Debug.LogWarning($"Префаб с ID {prefabId} не найден");
        return null;
    }

    private void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            SaveGame();
            saveTimer = 0f;
        }
    }

    public void SaveGame()
    {
        // Сохраняем базовые данные
        currentGameData.money = money.money;
        currentGameData.lvlPlayer = GameController.lvlPlayer;
        
        // Сохраняем состояние животных
        SaveAnimalStates();
        
        // Сохраняем постройки
        SaveBuildings();
        
        saveLoadManager.SaveGame(currentGameData);
        Debug.Log($"Данные сохранены: Деньги={currentGameData.money}, Уровень={currentGameData.lvlPlayer}");
    }

    private void SaveAnimalStates()
    {
        if (money.cow != null)
        {
            currentGameData.isCowUnlocked = money.cow.GetComponent<CheckLvlPlayer>().isOpenAnimal;
        }
        
        if (money.lama != null)
        {
            currentGameData.isLamaUnlocked = money.lama.GetComponent<CheckLvlPlayer>().isOpenAnimal;
        }
    }

    private void SaveBuildings()
    {
        // Находим все объекты с тегом PlaceBuild в сцене
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("PlaceBuild");
        List<BuildingData> buildingsData = new List<BuildingData>();

        foreach (GameObject building in buildings)
        {
            // Получаем компонент PrefabID из родительского объекта, если он там
            PrefabID prefabID = building.GetComponentInParent<PrefabID>();
            if (prefabID == null)
            {
                // Если не нашли в родителе, ищем в дочерних объектах
                prefabID = building.GetComponentInChildren<PrefabID>();
            }

            if (prefabID != null)
            {
                BuildingData data = new BuildingData
                {
                    position = building.transform.position,
                    prefabId = prefabID.id,
                    tag = building.tag
                };
                buildingsData.Add(data);
                Debug.Log($"Сохранена постройка: ID={prefabID.id}, Position={building.transform.position}");
            }
            else
            {
                Debug.LogWarning($"PrefabID не найден ни на объекте {building.name}, ни в его дочерних объектах");
            }
        }

        currentGameData.buildings = buildingsData.ToArray();
        Debug.Log($"Всего сохранено построек: {buildingsData.Count}");
    }
}