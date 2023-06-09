using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteDamage : MonoBehaviour
{

    private int damage = 35;
    private float range = 10f;
    [SerializeField] private Camera myCamera;
    public LayerMask EnemyLayer;

   
    private float buttonHoldTimer = 0f;
    private float requiredHoldTime = 3f;
   
   

    // Update is called once per frame
    void Update()
    {

        if (SaveScript.hasCursedObject == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(myCamera.transform.position, myCamera.transform.forward, out hit, range, EnemyLayer))
            {
                Debug.Log("raycastbulducanoyu");

                if (Input.GetMouseButton(0))
                {
                    Debug.Log("canavarsoltuþ");
                    buttonHoldTimer += Time.deltaTime;
                    Debug.Log(buttonHoldTimer);
                    if (buttonHoldTimer >= requiredHoldTime)
                    {
                        Debug.Log("Sol tuþa 3 saniye basýldý!");
                        Shoot(hit);
                        buttonHoldTimer = 0f;
                    }
                }
             

            }
        }




    }
    void Shoot(RaycastHit hit)
    {
        
            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if(enemyHealth != null )
            {
                enemyHealth.TakeDamage(damage);
                PlayerPickUp.Instance.Drop();

            }


    }
 
}
