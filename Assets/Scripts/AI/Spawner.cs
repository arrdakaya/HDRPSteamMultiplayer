using Mirror;
using System.Collections;
using UnityEngine;


public class Spawner : NetworkBehaviour
{
    public GameObject[] Items;
    private Transform Canvas;
    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.Find("SceneCanvas").transform;
        CmdSpawnObjects();
    }


    [Command(requiresAuthority = false)]
    void CmdSpawnObjects()
    {
        RpcSpawnObjects();
    }
    [ClientRpc]
    void RpcSpawnObjects()
    {
        StartCoroutine(Spawn());

    }
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2);
        GameObject timer = Instantiate(Items[0], Items[0].transform.position, Items[0].transform.rotation, Canvas);
        NetworkServer.Spawn(timer);
        for (int i = 1; i < Items.Length; i++)
        {
            GameObject spawnObject = Instantiate(Items[i], Items[i].transform.position, Items[i].transform.rotation);
            NetworkServer.Spawn(spawnObject);
        }


    }
}
