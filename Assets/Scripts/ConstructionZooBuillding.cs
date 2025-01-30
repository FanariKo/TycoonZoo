using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionZooBuillding : MonoBehaviour
{
    public GameObject nowBuild; // ������, ������� ����� ���������� ��� �����
    public GameObject buildPref;
    public GameObject[] prefabVariants;
    public static int prefabID;

    private GameObject currentOpenMenu = null; // ������� �������� ����

    // Кэшируем компоненты камеры для оптимизации
    private Camera mainCamera;
    private int spawnedObjectLayer;
    
    void Start()
    {
        mainCamera = Camera.main;
        spawnedObjectLayer = LayerMask.NameToLayer("SpawnedObject");
    }

    void Update()
    {
        GameController.playerMoney = PlayerPrefs.GetInt("Money");
        SetActiveNowBuild();
        
        // Объединяем проверки для оптимизации
        if (Input.GetMouseButtonDown(0))
        {
            if (prefabID != 0)
            {
                CreateNewAnimal();
            }
            DestroyAnimal();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            CallBuildMenu();
        }
    }

    public void CreateNewAnimal()
    {
        if (!buildPref.GetComponent<CheckLvlPlayer>().isOpenAnimal) return;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (!Physics.Raycast(ray, out hit)) return;
        
        PrefabID prefabIDComponent = buildPref.GetComponent<PrefabID>();
        if (prefabIDComponent == null) return;
        
        if (hit.collider.CompareTag("FreePlaceBuild") && GameController.playerMoney >= prefabIDComponent.priceBuild)
        {
            SpawnNewBuilding(hit, prefabIDComponent);
        }
    }

    private void SpawnNewBuilding(RaycastHit hit, PrefabID prefabIDComponent)
    {
        Vector3 spawnPosition = hit.transform.position + new Vector3(0, 0.45f, 0);
        GameObject spawnedPrefab = Instantiate(buildPref, spawnPosition, Quaternion.identity);
        spawnedPrefab.transform.SetParent(hit.transform);

        hit.collider.tag = "PlaceBuild";
        GameController.playerMoney -= prefabIDComponent.priceBuild;
        PlayerPrefs.SetInt("Money", GameController.playerMoney);
    }

    public void CallBuildMenu()
    {
        // ��������� ������� ������ ������ ����
        if (Input.GetMouseButtonDown(1))
        {
            if (!ChoiseBuild.isBuildChosen) 
            { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ������� ����� �����, �������� ���� "SpawnedObject"
            int layerMask = ~(1 << LayerMask.NameToLayer("SpawnedObject"));

                // ���������, ����� �� ��� � ������, ��������� ���� "SpawnedObject"
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.CompareTag("PlaceBuild"))
                    {
                        Transform parent = hit.transform;
                        Debug.Log("OpenBuildMenu"); // �������� ���������� ���������

                        foreach (Transform child in parent)
                        {
                            if (child.CompareTag("MenuBuild"))
                            {
                                // ���� ���� ��� �������, ��������� ���
                                if (currentOpenMenu != null && currentOpenMenu == child.gameObject)
                                {
                                    child.gameObject.SetActive(false);
                                    currentOpenMenu = null;
                                }
                                else
                                {
                                    // ��������� ���������� ����, ���� ��� �������
                                    if (currentOpenMenu != null)
                                    {
                                        currentOpenMenu.SetActive(false);
                                    }

                                    // ��������� ����� ����
                                    child.gameObject.SetActive(true);
                                    currentOpenMenu = child.gameObject;
                                }
                            }
                        }
                    }
                    else if (currentOpenMenu != null)
                    {
                        // ��������� ����, ���� ���� ��� ��� ������� � ����� "PlaceBuild"
                        currentOpenMenu.SetActive(false);
                        currentOpenMenu = null;
                    }
                }
            }
        }
    }

    void DestroyAnimal()
    {
        // ��������� ������� ����� ������ ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���������, ����� �� ��� � ������
            if (Physics.Raycast(ray, out hit))
            {
                // ���������, ��� � ������� ���� ��� "DestroyButton"
                if (hit.collider.CompareTag("DestroyButton"))
                {
                    Debug.Log("Build Destroy");

                    // �������� ������������ ������ MenuBuild
                    GameObject menuBuild = hit.transform.parent.gameObject;

                    // �������� ������� ������ Hill-A (�������� MenuBuild)
                    GameObject build = menuBuild.transform.parent.gameObject;

                    // ���� ������ � ����� "Zoo" ����� �������� �������� Hill-A
                    foreach (Transform child in build.transform)
                    {
                        if (child.CompareTag("Zoo")) // ���������, ��� ������ ����� ��� "Zoo"
                        {
                            // ���������� ������ � ����� "Zoo"
                            Destroy(child.gameObject);
                            Debug.Log("Chicken(Clone) destroyed");
                            menuBuild.gameObject.SetActive(false);
                            break; // ��������� ���� ����� �����������
                        }
                    }

                    // ���� �����, ����� ����� �������� ��� ������������� ������� ������� �� "FreePlaceBuild"
                    build.tag = "FreePlaceBuild";
                }
            }
        }
    }

    void SetActiveNowBuild()
    {
        ChoiseBuild.isBuildChosen = false;
        prefabID = 0;

        foreach (Transform child in nowBuild.transform)
        {
            if (!child.gameObject.activeSelf) continue;
            
            if (TryGetPrefabID(child, out PrefabID prefabIDComponent))
            {
                prefabID = prefabIDComponent.id;
                if (TryFindMatchingPrefab(prefabID))
                {
                    ChoiseBuild.isBuildChosen = true;
                    return;
                }
            }
        }
    }

    private bool TryGetPrefabID(Transform child, out PrefabID prefabIDComponent)
    {
        prefabIDComponent = child.GetComponent<PrefabID>();
        return prefabIDComponent != null;
    }

    private bool TryFindMatchingPrefab(int targetID)
    {
        foreach (var prefab in prefabVariants)
        {
            PrefabID prefabVariantData = prefab.GetComponent<PrefabID>();
            if (prefabVariantData != null && prefabVariantData.id == targetID)
            {
                buildPref = prefab;
                return true;
            }
        }
        return false;
    }
}