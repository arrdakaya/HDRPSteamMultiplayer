using Mirror;

using UnityEngine;
public class DoorType : NetworkBehaviour
{
    public enum typeOfDoor
    {
        room,
        passwordDoor
    }
    public typeOfDoor chooseDoor;

    public bool opened = false;
    public bool locked = false;
    [HideInInspector]
    public string message = "";
    private NetworkAnimator netAnim;

    // Start is called before the first frame update
    void Start()
    {
        netAnim = GetComponent<NetworkAnimator>();
        if (opened == true)
        {
            netAnim.SetTrigger("Open");
            message = "Press E to close the door";
        }
    }
}
