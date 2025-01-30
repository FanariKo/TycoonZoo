using UnityEngine;

public class PrefabID : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("Уникальный идентификатор префаба")]
    public int id;
    
    [Tooltip("Стоимость постройки")]
    public int priceBuild;
    
    [Tooltip("Цена за посещение")]
    public int visitPrise;

     [Tooltip("Процент счастья")]
    public int happyPercent;
}
