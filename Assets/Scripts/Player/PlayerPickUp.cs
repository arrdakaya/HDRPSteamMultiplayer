
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PlayerPickUp : NetworkBehaviour
{
    private RaycastHit hit;
    public LayerMask includeLayer;
    public GameObject pickupPanel;

    public Image mainImage;
    public Sprite[] itemIcons;
    public TextMeshProUGUI mainTitle;
    public string[] itemTitles;

    private int objID = 0;

    private void Start()
    {
        pickupPanel.SetActive(false);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.T))
        {
            CutsceneManager.Instance.CmdSwitchToCutscene();
        }
        if (Physics.SphereCast(transform.position,0.5f,transform.forward,out hit, 30.0f, includeLayer))
        {
            if(Vector3.Distance(transform.position,hit.transform.position) < 6)
            {
                if (hit.transform.gameObject.CompareTag("Item"))
                {
                    pickupPanel.SetActive(true);
                    objID = (int)hit.transform.gameObject.GetComponent<ItemTypes>().chooseItem;
                    mainImage.sprite = itemIcons[objID];
                    mainTitle.text = itemTitles[objID];
                    Debug.Log(hit.collider.name);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                      
                        SaveScript.itemAmts[objID]++;
                        CmdDestroyItem(hit.transform.gameObject);
                    }
                }
                
            }
           
        }
        else
        {
            if (pickupPanel.activeSelf == true)
            {
                Debug.Log("panelkapan");
                pickupPanel.SetActive(false);
            }

        }



    }
    [Command(requiresAuthority = false)]
    void CmdDestroyItem(GameObject obj)
    {
        RpcDestroyItem(obj);
    }
    [ClientRpc]
    void RpcDestroyItem(GameObject obj)
    {
        Destroy(obj, 0.2f);
    }

}

