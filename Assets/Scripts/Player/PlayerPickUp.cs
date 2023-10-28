
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
    private GameObject myWeapon;
    public GameObject cursedWeapon;

    private int objID = 0;
    private AudioSource audioPlayer;
    public GameObject doorMessageObj;
    public TextMeshProUGUI doorMessage;
    public AudioClip[] pickupSounds;

    public GameObject playerHand;

    GameObject myDoor;
    PlayerMovementController playerMovementController;
    NetworkAnimator myDoorNetworkAnimator = null;
    DoorType myDoorType = null;

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
        myWeapon = playerHand.transform.GetChild(0).gameObject;
        playerMovementController = GetComponent<PlayerMovementController>();
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
                        hit.transform.gameObject.GetComponent<NetworkAnimator>().SetTrigger("Open");
                    }
                    else if (hit.transform.gameObject.GetComponent<DoorType>().opened == true)
                    {
                        hit.transform.gameObject.GetComponent<DoorType>().message = " Press E to open the door";
                        hit.transform.gameObject.GetComponent<DoorType>().opened = false;
                        hit.transform.gameObject.GetComponent<NetworkAnimator>().SetTrigger("Close");
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("PasswordDoor"))
            {
                SaveScript.doorObject = hit.transform.gameObject;
                myDoor = hit.transform.gameObject;

                if (myDoor != null)
                {
                    if (myDoorNetworkAnimator == null && myDoorType == null)
                    {
                        myDoorNetworkAnimator = myDoor.GetComponent<NetworkAnimator>();
                        myDoorType = myDoor.GetComponent<DoorType>();
                    }
                    if (myDoorNetworkAnimator != null && myDoorType != null)
                    {
                        if (myDoorType.locked == true)
                        {
                            doorMessage.text = "Password Door. Press E to enter password";
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                playerMovementController.canMove = false;
                                playerMovementController.canCameraMove = false;

                                if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
                                {
                                    myDoor.GetComponent<KeyPad>().OpenKeyUI();
                                }
                            }
                        }
                        if (myDoor.GetComponent<KeyPad>().isPasswordCorrect == true)
                        {
                            myDoorType.locked = false;
                            playerMovementController.canMove = true;
                            playerMovementController.canCameraMove = true;
                        }

                        if (Input.GetKeyDown(KeyCode.E) && myDoorType.locked == false)
                        {
                            audioPlayer.clip = pickupSounds[0];
                            audioPlayer.Play();

                            if (myDoorType.opened == false)
                            {
                                myDoorType.message = " Press E to close the door";
                                myDoorType.opened = true;
                                myDoorNetworkAnimator.SetTrigger("Open");
                            }
                            else if (myDoorType.opened == true)
                            {
                                myDoorType.message = " Press E to open the door";
                                myDoorType.opened = false;
                                myDoorNetworkAnimator.SetTrigger("Close");
                            }
                        }
                    }
                    objID = (int)myDoorType.chooseDoor;
                    doorMessageObj.SetActive(true);
                    doorMessage.text = myDoorType.message;



                    if (Input.GetMouseButtonDown(1))
                    {
                        myDoor.GetComponent<KeyPad>().CloseKeyUI();
                        playerMovementController.canMove = true;
                        playerMovementController.canCameraMove = true;
                        myDoorNetworkAnimator = null;
                        myDoorType = null;
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
    }

    private void Pickup()
    {
        currentWeapon = hit.transform.gameObject;
        CmdPickUp(currentWeapon);

    }
    [Command]
    void CmdPickUp(GameObject currentWeapon)
    {
        RpcPickUp(currentWeapon);
    }
    [ClientRpc]
    void RpcPickUp(GameObject currentWeapon)
    {
        myWeapon.SetActive(true);
        CmdDestroyItem(currentWeapon);
        SaveScript.hasCursedObject = true;
    }
    public void Drop()
    {
        CmdDrop();
    }
    [Command]
    void CmdDrop()
    {
        RpcDrop();
    }
    [ClientRpc]
    void RpcDrop()
    {
        if (myWeapon.activeSelf == true)
        {
            myWeapon.SetActive(false);
            SaveScript.hasCursedObject = false;
            if (isServer)
            {
                GameObject droppedWeapon = Instantiate(cursedWeapon, playerHand.transform.position, playerHand.transform.rotation);
                NetworkServer.Spawn(droppedWeapon);
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
        NetworkServer.Destroy(obj);

    }

}

