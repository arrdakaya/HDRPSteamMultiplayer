using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorType : MonoBehaviour
{
    public enum typeOfDoor
    {
        room
    }
    public typeOfDoor chooseDoor;

    public bool opened = false;
    public bool locked = false;
    [HideInInspector]
    public string message = "Press E to open the door";
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (opened == true)
        {
            anim.SetTrigger("Open");
            message = "Press E to close the door";
        }
    }
}