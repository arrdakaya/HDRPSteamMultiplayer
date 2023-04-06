using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    public GameObject itemInventory;
    public Image healthImage;
    public Image batteryImage;
    public TextMeshProUGUI healthAmountText;
    public TextMeshProUGUI batteryAmountText;

    private float lerpSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (itemInventory.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.B))
            {
                StartCoroutine("ShowItemInventory");
            }
        }
        

        healthAmountText.text = "x" + SaveScript.itemAmts[1];
        batteryAmountText.text = "x" + SaveScript.itemAmts[0];
    }

    IEnumerator ShowItemInventory()
    {
        Color32 lerpedColor = new Color32(255, 255, 255, 80);
        float currentTime = 0;
        lerpedColor = Color.Lerp(new Color32(255, 255, 255, 80), new Color32(255, 100, 100, 80), Mathf.PingPong(currentTime += (Time.deltaTime * lerpSpeed / 1),1));
        itemInventory.SetActive(true);

        if (SaveScript.itemAmts[0] == 0)
        {
            batteryImage.color = lerpedColor;
        }
        if (SaveScript.itemAmts[1] == 0)
        {
            healthImage.color = lerpedColor;
        }

        //batteryImage.color = Color.Lerp(new Color32(255, 100, 100, 80), new Color32(255, 255, 255, 80), 2);
        //healthImage.color = Color.Lerp(new Color32(255, 100, 100, 80), new Color32(255, 255, 255, 80), 2);

        yield return new WaitForSeconds(10);
        itemInventory.SetActive(false);

    }
}
