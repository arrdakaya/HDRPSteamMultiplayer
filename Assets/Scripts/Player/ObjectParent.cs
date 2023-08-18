using Mirror;

using UnityEngine;

public class ObjectParent : NetworkBehaviour
{
    [SyncVar] public GameObject parent;

    void Update()
    {
        if (parent != null)
        {

            transform.position = parent.transform.position;
            transform.rotation = parent.transform.rotation;
            transform.parent = parent.transform;

        }

    }
   
   
}
