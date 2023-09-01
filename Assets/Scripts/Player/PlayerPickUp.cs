
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using StarterAssets;

public class PlayerPickUp : NetworkBehaviour
{
    public static PlayerPickUp Instance;

    private RaycastHit hit;
    public LayerMask includeLayer;
    public GameObject pickupPanel;

    [Header("Item Features")]
    public Image mainImage;
    public Sprite[] itemIcons;
    public TextMeshProUGUI mainTitle;
    public string[] itemTitles;


    [Header("Weapon Features")]
    public Sprite[] weaponIcons;
    public string[] weaponTitles;
    public GameObject currentWeapon;

    private int objID = 0;
    private AudioSource audioPlayer;
    public GameObject doorMessageObj;
    public TextMeshProUGUI doorMessage;
    public AudioClip[] pickupSounds;

    public GameObject playerHand;

    GameObject myDoor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        doorMessageObj.SetActive(false);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (Camera.main == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 2, includeLayer))
        {

            if (hit.transform.gameObject.CompareTag("Item"))
            {
                pickupPanel.SetActive(true);
                objID = (int)hit.transform.gameObject.GetComponent<ItemTypes>().chooseItem;
                mainImage.sprite = itemIcons[objID];
                mainTitle.text = itemTitles[objID];
                if (Input.GetKeyDown(KeyCode.E))
                {
                   
                    SaveScript.itemAmts[objID]++;
                    CmdDestroyItem(hit.transform.gameObject);
                }
            }
            else if (hit.transform.gameObject.CompareTag("Weapon"))
            {
                pickupPanel.SetActive(true);
                objID = (int)hit.transform.gameObject.GetComponent<WeaponTypes>().chooseWeapon;
                mainImage.sprite = weaponIcons[objID];
                mainTitle.text = weaponTitles[objID];
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (currentWeapon == null)
                    {
                        Pickup();
                    }
                    else
                    {
                        Drop();
                        if (currentWeapon == null)
                            Pickup();
                    }
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

                if (hit.transform.gameObject.GetComponent<DoorType>().locked == true)
                {
                    doorMessage.text = "Locked. Find the key";
                }

                if (Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<DoorType>().locked == false)
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
            else if (hit.transform.gameObject.CompareTag("PasswordDoor"))
            {
                SaveScript.doorObject = hit.transform.gameObject;
                objID = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;
                doorMessageObj.SetActive(true);
                doorMessage.text = hit.transform.gameObject.GetComponent<DoorType>().message;

                myDoor = hit.transform.gameObject;
                if (myDoor.GetComponent<DoorType>().locked == true)
                {
                    doorMessage.text = "Password Door. Press E to enter password";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        gameObject.GetComponent<PlayerMovementController>().canMove = false;
                        gameObject.GetComponent<PlayerMovementController>().canCameraMove = false;

                        if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
                        {
                            myDoor.GetComponent<KeyPad>().OpenKeyUI();
                            
                        }

                    }
                }
                if (myDoor.GetComponent<KeyPad>().isPasswordCorrect == true)
                {
                    myDoor.GetComponent<DoorType>().locked = false;
                    gameObject.GetComponent<PlayerMovementController>().canMove = true;
                    gameObject.GetComponent<PlayerMovementController>().canCameraMove = true;


                }

                if (Input.GetKeyDown(KeyCode.E) && myDoor.GetComponent<DoorType>().locked == false)
                {
                    audioPlayer.clip = pickupSounds[0];
                    audioPlayer.Play();

                    if (myDoor.GetComponent<DoorType>().opened == false)
                    {
                        myDoor.GetComponent<DoorType>().message = " Press E to close the door";
                        myDoor.GetComponent<DoorType>().opened = true;
                        myDoor.GetComponent<Animator>().SetTrigger("Open");
                    }
                    else if (myDoor.GetComponent<DoorType>().opened == true)
                    {
                        myDoor.GetComponent<DoorType>().message = " Press E to open the door";
                        myDoor.GetComponent<DoorType>().opened = false;
                        myDoor.GetComponent<Animator>().SetTrigger("Close");
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
            if (doorMessageObj.activeSelf == true)
            {
                doorMessageObj.SetActive(false);
                SaveScript.doorObject = null;
            }

        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Drop();
        }
        if(myDoor != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                myDoor.GetComponent<KeyPad>().CloseKeyUI();
                gameObject.GetComponent<PlayerMovementController>().canMove = true;
                gameObject.GetComponent<PlayerMovementController>().canCameraMove = true;

            }
        }
        if(currentWeapon != null)
        {
            currentWeapon.transform.position = playerHand.transform.position;
            currentWeapon.transform.rotation = playerHand.transform.rotation;
        }
    }
        
   

    private void Pickup()
    {
            currentWeapon = hit.transform.gameObject;
            CmdPickUp(currentWeapon);   
        
    }
    [Command]
    void CmdPickUp(GameObject currentWeapon)
    {
        currentWeapon.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
        //currentWeapon.transform.position = weaponParent.transform.position;
        //currentWeapon.transform.parent = weaponParent.transform;
        //currentWeapon.GetComponent<ObjectParent>().parent = playerHand;
        currentWeapon.transform.position = playerHand.transform.position;
        currentWeapon.transform.rotation = playerHand.transform.rotation;
        currentWeapon.tag = "PlayerWeapon";
        SaveScript.hasCursedObject = true;

    }
    public void Drop()
    {
        CmdDrop();
    }
    [Command]
    void CmdDrop()
    {
        if (currentWeapon != null)
        {
            currentWeapon.transform.parent = null;
            currentWeapon.GetComponent<ObjectParent>().parent = null;
            currentWeapon.transform.GetComponent<Rigidbody>().isKinematic = false;
            currentWeapon.tag = "Weapon";
            currentWeapon = null;
            SaveScript.hasCursedObject = false;

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
        NetworkServer.Destroy(obj);
        
    }

}

