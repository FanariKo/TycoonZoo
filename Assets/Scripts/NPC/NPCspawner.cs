using UnityEngine;
using System.Collections;

public class NPCspawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] points;
    [SerializeField] private float minSpawnDelay = 2f;
    [SerializeField] private float maxSpawnDelay = 5f;
    
    private int currentMaxNPCs = 10; // Начальный лимит для 1 уровня

    private void Start()
    {
        UpdateNPCLimit();
        if (ValidateComponents())
        {
            StartCoroutine(SpawnNPCs());
        }
    }

    private void UpdateNPCLimit()
    {
        int playerLevel = PlayerPrefs.GetInt("LvlPlayer", 1);
        currentMaxNPCs = playerLevel switch
        {
            1 => 10,
            2 => 20,
            3 => 30,
            _ => 10
        };
    }

    private bool ValidateComponents()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("NPC Prefab не назначен в NPCspawner");
            return false;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn Point не назначен в NPCspawner");
            return false;
        }
        if (points == null || points.Length == 0)
        {
            Debug.LogError("Points не назначены в NPCspawner");
            return false;
        }
        return true;
    }

    private IEnumerator SpawnNPCs()
    {
        while (true)
        {
            UpdateNPCLimit(); // Обновляем лимит при каждой итерации
            if (GetCurrentNPCCount() < currentMaxNPCs)
            {
                SpawnNPC();
            }
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void SpawnNPC()
    {
        GameObject npc = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        NPCMove npcMove = npc.GetComponent<NPCMove>();
        
        if (npcMove != null)
        {
            npcMove.points = points;
        }
        else
        {
            Debug.LogError("NPCMove component not found on spawned NPC");
        }
    }

    private int GetCurrentNPCCount()
    {
        return GameObject.FindGameObjectsWithTag("NPC").Length;
    }
}