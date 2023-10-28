using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteDamage : NetworkBehaviour
{
    private Animator anim;
    private int CursedObjectLayerIndex;

    private int damage = 50;
    private float range = 10f;
    [SerializeField] private Camera myCamera;
    public LayerMask EnemyLayer;


    private float buttonHoldTimer = 0f;
    private float requiredHoldTime = 3f;

    void Start()
    {
        if (!isLocalPlayer) { return; }
        anim = GetComponent<Animator>();
        CursedObjectLayerIndex = anim.GetLayerIndex("CursedItem");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
        if (SaveScript.hasCursedObject == true)
        {
            anim.SetLayerWeight(CursedObjectLayerIndex, 1f);
            RaycastHit hit;
            if (Physics.Raycast(myCamera.transform.position, myCamera.transform.forward, out hit, range, EnemyLayer))
            {
                if (Input.GetMouseButton(0))
                {
                    buttonHoldTimer += Time.deltaTime;
                    Debug.Log(buttonHoldTimer);
                    if (buttonHoldTimer >= requiredHoldTime)
                    {
                        Debug.Log("Sol tuþa 3 saniye basýldý!");
                        Shoot(hit);
                        buttonHoldTimer = 0f;
                    }
                }
                else
                {
                    buttonHoldTimer = 0f;
                }
            }

        }
        else
        {
            anim.SetLayerWeight(CursedObjectLayerIndex, 0f);

        }
    }
    void Shoot(RaycastHit hit)
    {

        EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            PlayerPickUp.Instance.Drop();

        }

    }

}
