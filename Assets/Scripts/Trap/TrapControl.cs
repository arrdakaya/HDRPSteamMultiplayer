using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapControl : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<NetworkAnimator>().animator; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("TrapTrigger");
            other.GetComponent<Health>().DecreaseHP(50);
            StartCoroutine("TrapDestroy");
         
        }
       
    }
    IEnumerator TrapDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
