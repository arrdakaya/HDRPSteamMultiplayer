using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrapController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rb.useGravity = true;
        }
    }
}
