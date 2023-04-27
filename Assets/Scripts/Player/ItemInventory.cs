using TMPro;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public TextMeshProUGUI healthAmountText;
    public TextMeshProUGUI batteryAmountText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthAmountText.text = "x" + SaveScript.itemAmts[1];
        batteryAmountText.text = "x" + SaveScript.itemAmts[0];
    }

  
}
