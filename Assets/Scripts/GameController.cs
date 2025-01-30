using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Player Stats")]
    public int money;
    public static int playerMoney;
    public static int lvlPlayer;
    public int nowLvlPlayer;
    public AudioSource lvlUp_audio;

    [Header("Animals")]
    public GameObject cow;
    public GameObject lama;

    [Header("UI Elements")]
    public Text lvlText;

    [Header("Visual Settings")]
    public Color openColor = Color.white;
    public Color closeColor = Color.black;

    private void Start()
    {
        lvlPlayer = PlayerPrefs.GetInt("LvlPlayer", 1);
        InitializeUI();
        InitializeAnimals();
        SyncMoneyWithPlayerPrefs();
    }

    private void Update()
    {    
        SyncMoney();
        LevelUp();
        NpcStats();
    }

    private void InitializeUI()
    {
        lvlText.text = PlayerPrefs.GetInt("LvlPlayer").ToString();
        nowLvlPlayer = PlayerPrefs.GetInt("LvlPlayer");
    }

    private void InitializeAnimals()
    {
        if (nowLvlPlayer < 2)
        {
            SetAnimalState(cow, false);
        }
            
        if (nowLvlPlayer < 3)
        {
            SetAnimalState(lama, false);
        }
    }

    private void SetAnimalState(GameObject animal, bool isOpen)
    {
        if (animal != null)
        {
            animal.GetComponent<CheckLvlPlayer>().isOpenAnimal = isOpen;
            Renderer renderer = animal.GetComponent<Renderer>();
            renderer.sharedMaterial.color = isOpen ? openColor : closeColor;
        }
    }

    private void SyncMoneyWithPlayerPrefs()
    {
        playerMoney = money;
        PlayerPrefs.SetInt("Money", money);
    }

    private void SyncMoney()
    {
        money = playerMoney;
    }

    public void LevelUp()
    {
        lvlText.text = PlayerPrefs.GetInt("LvlPlayer").ToString();
        CheckLevelUpConditions();
        OpenNewAnimal();
    }

    private void CheckLevelUpConditions()
    {
        if (playerMoney >= 500 && lvlPlayer < 2)
        {
            lvlUp_audio.Play();
            lvlPlayer = 2;
            Debug.Log("Level Up, Open Cow");
            PlayerPrefs.SetInt("LvlPlayer", lvlPlayer);
        }

        if (playerMoney >= 1500 && lvlPlayer < 3)
        {
            lvlUp_audio.Play();
            lvlPlayer = 3;
            Debug.Log("Level Up, Open Lama");
            PlayerPrefs.SetInt("LvlPlayer", lvlPlayer);
        }
    }

    public void OpenNewAnimal()
    {
        if (lvlPlayer == 2)
        {
            SetAnimalState(cow, true);
        }
        if (lvlPlayer == 3)
        {
            SetAnimalState(lama, true);
        }
    }

    private void NpcStats()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("NPC"))
                {
                    Transform npcTransform = hit.transform;
                    
                    // Проверяем текущее состояние первого дочернего объекта
                    bool isActive = npcTransform.GetChild(0).gameObject.activeSelf;
                    
                    // Переключаем состояние всех дочерних объектов
                    foreach (Transform child in npcTransform)
                    {
                        child.gameObject.SetActive(!isActive);
                        Debug.Log($"NPC Stats {(!isActive ? "activated" : "deactivated")}");
                    }
                }
            }
        }
    }
}
