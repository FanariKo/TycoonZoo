using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NpcStat : MonoBehaviour
{
    [Header("NPC Stats")]
    [SerializeField] private TextMeshPro[] happyTextComponents;
    private int happyCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zoo"))
        {
            PrefabID animalStats = other.GetComponent<PrefabID>();
            if (animalStats != null)
            {
                // Увеличиваем уровень счастья в зависимости от животного
                happyCount += animalStats.happyPercent;
                UpdateHappyText();
                Debug.Log($"NPC счастье увеличено на {animalStats.happyPercent}. Текущий уровень: {happyCount}");
            }
            else
            {
                Debug.LogWarning("Компонент PrefabID не найден на объекте Zoo!");
            }
        }
    }

    private void UpdateHappyText()
    {
        if (happyTextComponents != null && happyTextComponents.Length > 0)
        {
            foreach (TextMeshPro textComponent in happyTextComponents)
            {
                if (textComponent != null)
                {
                    textComponent.text = happyCount.ToString();
                }
            }
        }
    }

    public int GetHappyCount()
    {
        return happyCount;
    }
}
