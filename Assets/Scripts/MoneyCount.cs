using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyCount : MonoBehaviour
{
    private Text moneyText;

    private void Awake()
    {
        moneyText = GetComponent<Text>();
    }

    private void Update()
    {
        moneyText.text = PlayerPrefs.GetInt("Money").ToString();
    }
}
