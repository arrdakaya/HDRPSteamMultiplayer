using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TrapPlacer : NetworkBehaviour
{
    public static TrapPlacer instance;
    private bool groundTrapCreated;
    Ray ray;
    RaycastHit hitInfo;

    public LayerMask layerMask;
    //public LayerMask trapMask;
    public LayerMask PlayerLayer;
    [SerializeField] private float distance = 10f;
    private GameObject previewObject;
    public bool isPlacingObject;

    private int publicTrapNumber;
    private GameObject InstantiatedObject;
    public GameObject parasiteSpawn;
    public GameObject objectToPlace2;
    public GameObject placeTeleport;
    public GameObject groundTrap;

    public GameObject blueprint;
    public GameObject blueprint2;
    public GameObject teleportBlueprint;
    public GameObject groundTrapBlueprint;
    public GameObject destroyGroundTrapBlueprint;


    private GameObject groundTrapDestroy;

    public bool isOverlapping = false;
    public GameObject FailText;
    public GameObject SkillMenu;

    private bool isPlayerBlind = false;
    private GameObject blindPlayer;
    public GameObject blindnessOnPlayerIcon;

    [Header("Trap1")]
    public Image trapImage1;
    float cooldown1 = 10;
    bool isCooldown = false;

    [Header("Trap2")]
    public Image trapImage2;
    float cooldown2 = 10;
    bool isCooldown2 = false;

    [Header("Teleport")]
    public Image teleportImage;
    float cooldown3 = 20;
    bool isCooldown3 = false;

    [Header("Blindness")]
    public Image blindImage;
    float cooldown4 = 10;
    bool isCooldown4 = false;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public override void OnStartLocalPlayer()
    {
        // "isLocalPlayer" �zelli�i burada "true" olarak ayarlan�r.
        base.OnStartLocalPlayer();
        Debug.Log("islocalplayer:" + isLocalPlayer);
        SkillMenu.SetActive(isLocalPlayer);
        trapImage1.fillAmount = 0;
        trapImage2.fillAmount = 0;
        teleportImage.fillAmount = 0;
        blindImage.fillAmount = 0;
        // Burada, yerel oyuncunun ba�lang�� i�lemleri ger�ekle�tirilebilir.
    }

    void Update()
    {
        if(isLocalPlayer)
        {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            
            if (Physics.Raycast(ray, out hitInfo, distance, PlayerLayer))
            {
                if (!isCooldown4)
                {
                    blindnessOnPlayerIcon.SetActive(true);
                    if (Input.GetMouseButtonDown(1) && isPlayerBlind == false)
                    {
                        if (hitInfo.collider.CompareTag("Player"))
                        {

                            blindPlayer = hitInfo.transform.gameObject;
                            isCooldown4 = true;
                            blindImage.fillAmount = 1;
                            CmdBlindPlayer();


                        }
                    }
                }
                else
                {
                    blindnessOnPlayerIcon.SetActive(false);

                }

            }
            else
            {
                blindnessOnPlayerIcon.SetActive(false);

            }




            FailText.SetActive(false);
                if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
                {
                    if (!isOverlapping)
                    {

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (isPlacingObject)
                            {
                                if (hitInfo.collider.CompareTag("Ground"))
                                {
                                    Quaternion hitRotation = hitInfo.transform.rotation;
                                    PlaceObject(hitInfo.point, hitRotation);

                                    if (publicTrapNumber == 1)
                                    {
                                        isCooldown = true;
                                        trapImage1.fillAmount = 1;
                                    }
                                    else if (publicTrapNumber == 2)
                                    {
                                        isCooldown2 = true;
                                        trapImage2.fillAmount = 1;
                                    }
                                    else if (publicTrapNumber == 3)
                                    {
                                        isCooldown3 = true;
                                        teleportImage.fillAmount = 1;
                                    }
                                    

                                    if (hitInfo.collider.name == "Plane" || hitInfo.collider.name == "Sphere")
                                        isPlacingObject = false;
                                }
                            }

                        }

                    }
                    else
                    {
                        FailText.SetActive(true);

                    }
                if (hitInfo.collider.CompareTag("GroundTrap"))
                    {
                        if (previewObject == null)
                        {
                            if (!groundTrapCreated)
                            {
                                groundTrapDestroy = Instantiate(groundTrapBlueprint, new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.transform.position.z), Quaternion.identity);
                                groundTrap.transform.position = groundTrapDestroy.transform.position;
                                groundTrap.transform.rotation = groundTrapDestroy.transform.rotation;
                                groundTrapCreated = true;

                            }
                            if (Input.GetMouseButtonDown(0))
                            {
                                destroyGroundTrapBlueprint = hitInfo.transform.gameObject;
                                PlaceGroundTrap(destroyGroundTrapBlueprint);
                                isPlacingObject = false;

                            }
                        }
                    }
                    else
                    {
                        Destroy(groundTrapDestroy);
                        groundTrapCreated = false;
                    }
                     

            }
            
               

        
        
        if (isPlacingObject)
        {

            ManageObjectBlueprint();

        }
            Trap1();
            Trap2();
            Teleport();
            Trap1Cooldown();
            Trap2Cooldown();
            TeleportCooldown();
            BlindnessCooldown();
        }
    }
    void ManageObjectBlueprint()
    {        
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            if (hitInfo.collider.CompareTag("Ground"))
            {
                if (previewObject != null)
                {
                    //if (Input.GetKey(KeyCode.E))
                    //{
                    //    previewObject.transform.Rotate(Vector3.up, 90 * Time.deltaTime);


                    //}
                    //if (Input.GetKeyDown(KeyCode.Q))
                    //{
                    //    previewObject.transform.Rotate(Vector3.up, -45);


                    //}
                    

                    previewObject.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                    previewObject.transform.rotation = hitInfo.transform.rotation;


                    if (Input.GetMouseButtonDown(1))
                    {
                        Destroy(previewObject);
                    }
                }
            }
           
        }



    }
    void PlaceGroundTrap(GameObject destroyableObject)
    {
        if (groundTrapDestroy != null)
        {
            CMDPlaceGroundTrap(destroyableObject);
  
        }
    }
    [Command]
    void CMDPlaceGroundTrap(GameObject destroyableObject)
    {

        RPCplaceGroundTrap(destroyableObject);

    }
    [ClientRpc]
    void RPCplaceGroundTrap(GameObject destroyableObject)
    {
        if (isServer)
        {
            GameObject obj = Instantiate(groundTrap, new Vector3(groundTrap.transform.position.x, groundTrap.transform.position.y, groundTrap.transform.position.z), groundTrap.transform.rotation);
            NetworkServer.Spawn(obj);
        }
        Destroy(groundTrapDestroy);
        Destroy(destroyableObject);
    }
   
  

    void PlaceObject(Vector3 position, Quaternion hitRotation)
    {

        if (previewObject != null)
        {

            if (publicTrapNumber == 1)
            {
                CmdPlaceObject(position, hitRotation);
                Destroy(previewObject);

            }
            if (publicTrapNumber == 2)
            {
                CmdPlaceObject2(position, hitRotation);
                Destroy(previewObject);

            }
            if (publicTrapNumber == 3)
            {
                CmdPlaceTeleport(position, hitRotation);
                Destroy(previewObject);

            }

            isPlacingObject = false;
            
        }

    }
    [Command]
    void CmdPlaceObject(Vector3 position, Quaternion hitRotation)
    {
            RPCPlaceObject(position, hitRotation); 
    }
    [ClientRpc]
    void RPCPlaceObject(Vector3 position, Quaternion hitRotation)
    {

        if (isServer) 
        {

            InstantiatedObject = Instantiate(parasiteSpawn, new Vector3(position.x, position.y + parasiteSpawn.transform.position.y, position.z), hitRotation);
                NetworkServer.Spawn(InstantiatedObject);

        }
    }
    
    [Command]
    void CmdPlaceObject2(Vector3 position, Quaternion hitRotation)
    {
        RPCPlaceObject2(position, hitRotation);
    }

    [ClientRpc]
    void RPCPlaceObject2(Vector3 position, Quaternion hitRotation)
    {
        if (isServer)
        {
                InstantiatedObject = Instantiate(objectToPlace2, new Vector3(position.x, position.y + objectToPlace2.transform.position.y, position.z), hitRotation);
                NetworkServer.Spawn(InstantiatedObject);

        }

    }
 
    [Command]
    void CmdPlaceTeleport(Vector3 position, Quaternion hitRotation)
    {
        RPCPlaceTeleport(position, hitRotation);
    }

    [ClientRpc]
    void RPCPlaceTeleport(Vector3 position, Quaternion hitRotation)
    {
        if (isServer)
        {
            InstantiatedObject = Instantiate(placeTeleport, new Vector3(position.x, position.y + placeTeleport.transform.position.y, position.z), hitRotation);
            NetworkServer.Spawn(InstantiatedObject);

        }
    }

    void Trap1()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isCooldown == false)
        {
            if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
            {
                if (hitInfo.collider.CompareTag("Ground"))
                {
                    if (previewObject == null)
                    {

                        previewObject = (GameObject)Instantiate(blueprint);
                        publicTrapNumber = 1;
                       
                    }
                    else if (previewObject != null)
                    {
                        Destroy(previewObject);
                        previewObject = (GameObject)Instantiate(blueprint);

                        publicTrapNumber = 1;

                        isOverlapping = false;
                        FailText.SetActive(false);
                    }
                    if (previewObject != null)
                    {
                        isPlacingObject = true;
                    }
            }   }
        }


    }
    void Trap2()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && isCooldown2 == false)
        {
            if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
            {
                if (hitInfo.collider.CompareTag("Ground"))
                {
                    if (previewObject == null)
                    {
                        previewObject = (GameObject)Instantiate(blueprint2);
                        publicTrapNumber = 2;

                    }
                    else if (previewObject != null)
                    {
                        Destroy(previewObject);
                        previewObject = (GameObject)Instantiate(blueprint2);

                        publicTrapNumber = 2;
                        isOverlapping = false;
                        FailText.SetActive(false);
                    }
                    if (previewObject != null)
                    {
                        isPlacingObject = true;
                    }
                }
            }   
        }


    }
    void Teleport()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && isCooldown3 == false)
        {
            if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
            {
                if (hitInfo.collider.CompareTag("Ground"))
                {
                    if (previewObject == null)
                    {
                        previewObject = (GameObject)Instantiate(teleportBlueprint);
                        publicTrapNumber = 3;
                    }
                    else if (previewObject != null)
                    {
                        Destroy(previewObject);
                        previewObject = (GameObject)Instantiate(teleportBlueprint);
                        publicTrapNumber = 3;
                        isOverlapping = false;
                        FailText.SetActive(false);
                    }
                    if (previewObject != null)
                    {
                        isPlacingObject = true;
                    }
                }
            }
        }


    }
   

    void Trap1Cooldown()
    {
        
            if (isCooldown)
            {
                trapImage1.fillAmount -= 1 / cooldown1 * Time.deltaTime;
                if (trapImage1.fillAmount <= 0)
                {
                    trapImage1.fillAmount = 0;
                    isCooldown = false;

                }
            }
        
        
    }
    void Trap2Cooldown()
    {
        
            if (isCooldown2)
            {
                trapImage2.fillAmount -= 1 / cooldown2 * Time.deltaTime;
                if (trapImage2.fillAmount <= 0)
                {
                    trapImage2.fillAmount = 0;
                    isCooldown2 = false;

            }
        }
        
        
    }
    void TeleportCooldown()
    {
        
            if (isCooldown3)
            {
                teleportImage.fillAmount -= 1 / cooldown3 * Time.deltaTime;
                if (teleportImage.fillAmount <= 0)
                {
                    teleportImage.fillAmount = 0;
                    isCooldown3 = false;

            }
    }
        
        
    }
    void BlindnessCooldown()
    {
        if (isCooldown4)
        {
            blindImage.fillAmount -= 1 / cooldown4 * Time.deltaTime;
            if (blindImage.fillAmount <= 0)
            {
                blindImage.fillAmount = 0;
                isCooldown4 = false;

            }
        }
    }
    [Command]
    void CmdBlindPlayer()
    {
        RpcBlindPlayer();
    }
    [ClientRpc]
    void RpcBlindPlayer()
    {
        StartCoroutine(BlindPlayer());

    }
    IEnumerator BlindPlayer()
    {


        blindPlayer.transform.GetChild(0).GetComponent<Volume>().enabled = true;
        isPlayerBlind = true;
        yield return new WaitForSeconds(2);
        blindPlayer.transform.GetChild(0).GetComponent<Volume>().enabled = false;
        yield return new WaitForSeconds(10);

        isPlayerBlind = false;
    }

}



