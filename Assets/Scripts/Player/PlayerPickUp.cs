
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Unity.VisualScripting;

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
    private AudioSource audioPlayer;
    public GameObject doorMessageObj;
    public TextMeshProUGUI doorMessage;
    public AudioClip[] pickupSounds;

    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        doorMessageObj.SetActive(false);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

       
        if (Physics.Raycast(ray, out hit, 2, includeLayer))
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
              
             else if (hit.transform.gameObject.CompareTag("Door"))
             {
                SaveScript.doorObject = hit.transform.gameObject;
                objID = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;
                doorMessageObj.SetActive(true);
                doorMessage.text = hit.transform.gameObject.GetComponent<DoorType>().message;

                if (SaveScript.itemAmts[2] != 0)
                {
                    hit.transform.gameObject.GetComponent<DoorType>().locked = false;
                }

                if(hit.transform.gameObject.GetComponent<DoorType>().locked == true)
                {
                        doorMessage.text = "Locked. Find the key";
                }

                if(Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<DoorType>().locked == false)
                {
                    audioPlayer.clip = pickupSounds[0];
                    audioPlayer.Play();

                    if (hit.transform.gameObject.GetComponent<DoorType>().opened == false)
                    {
                            hit.transform.gameObject.GetComponent<DoorType>().message = " Press E to close the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = true;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");
                    }
                    else if (hit.transform.gameObject.GetComponent<DoorType>().opened == true)
                    {
                            hit.transform.gameObject.GetComponent<DoorType>().message = " Press E to open the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = false;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");
                    }
                }
             }
               
        }
        else
        {
            if (pickupPanel.activeSelf == true)
            {
                pickupPanel.SetActive(false);
            }
            if(doorMessageObj.activeSelf == true)
            {
                doorMessageObj.SetActive(false);
                SaveScript.doorObject = null;
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

