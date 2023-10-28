using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapControl : MonoBehaviour
{
    private NetworkAnimator netAnim;
    private void Start()
    {
        netAnim = GetComponent<NetworkAnimator>(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            netAnim.SetTrigger("TrapTrigger");    
            if(other.GetComponent<PlayerAbilities>().GetLessDamageTrap())
            {
                other.GetComponent<Health>().DecreaseHP(30);

            }
            else
            {
                other.GetComponent<Health>().DecreaseHP(50);
            }
            StartCoroutine("TrapDestroy");
         
        }
    }
    IEnumerator TrapDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
