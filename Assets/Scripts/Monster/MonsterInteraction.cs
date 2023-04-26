using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class MonsterInteraction : NetworkBehaviour
{
    private RaycastHit hit;
    public LayerMask includeLayer;

    public GameObject doorMessageObj;
    public TextMeshProUGUI doorMessage;
    public AudioClip[] pickupSounds;

    private AudioSource audioPlayer;

    private int objID = 0;


    // Start is called before the first frame update
    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        doorMessageObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 2, includeLayer))
        {
            if (hit.transform.gameObject.CompareTag("Door"))
            {
                Debug.Log(hit.transform.gameObject);
                SaveScript.doorObject = hit.transform.gameObject;
                objID = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;
                doorMessageObj.SetActive(true);
                doorMessage.text = hit.transform.gameObject.GetComponent<DoorType>().message;


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
        }
        else
        {
            if (doorMessageObj.activeSelf == true)
            {
                doorMessageObj.SetActive(false);
                SaveScript.doorObject = null;
            }

        }
    }
}
