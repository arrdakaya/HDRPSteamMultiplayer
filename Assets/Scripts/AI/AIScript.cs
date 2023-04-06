using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{

    public enum AIType
    {
        catwalk
    }

    public AIType type;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetLayerWeight((int)type, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
