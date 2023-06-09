using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParasiteAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            other.GetComponent<Health>().DecreaseHP(20);
        }
    }
}
