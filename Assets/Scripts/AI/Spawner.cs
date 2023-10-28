using Mirror;
using System.Collections;
using UnityEngine;


public class Spawner : NetworkBehaviour
{
    public GameObject[] Items;
    private RectTransform Canvas;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            Canvas = GameObject.Find("SceneCanvas").GetComponent<RectTransform>();
            CmdSpawnObjects();

        }
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
        for (int i = 1; i < Items.Length; i++)
        {
            GameObject spawnObject = Instantiate(Items[i], Items[i].transform.position, Items[i].transform.rotation);
            NetworkServer.Spawn(spawnObject);
            GameObject healthtemp = Instantiate(Items[1], Items[1].transform.position, Items[1].transform.rotation);
            NetworkServer.Spawn(healthtemp);

        }


    }
}
