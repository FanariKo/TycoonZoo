using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiseBuild : MonoBehaviour
{
    public Transform shopItemsParent; // Родитель кнопок
    public Transform nowBuild;        // Родитель объектов
    public GameObject emptyBuild;
    public static bool isBuildChosen;
    private void Start()
    {
        isBuildChosen = false;
        foreach (Transform item in shopItemsParent)
        {
            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => ActivateGameObject(item.name));
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
           
                if (isBuildChosen)
                {
                    foreach (Transform child in nowBuild)
                    {
                        child.gameObject.SetActive(false);
                    }
                    nowBuild.gameObject.SetActive(true);
                    ConstructionZooBuillding.prefabID = 0;
                    isBuildChosen = false;
                }
            
           
        }
    }
    private void ActivateGameObject(string objectName)
    {
        // Деактивировать все объекты
        foreach (Transform child in nowBuild)
        {
            child.gameObject.SetActive(false);
        }

        // Активировать выбранный объект
        Transform targetObject = nowBuild.Find(objectName);
        if (targetObject != null)
        {
            targetObject.gameObject.SetActive(true);
        }
    }
}
