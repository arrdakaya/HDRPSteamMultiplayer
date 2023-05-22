using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

  
    public void FootStep()
    {
        
            AudioClip clip = GetRandomClip();
            audioSource.PlayOneShot(clip);
        
    }

    private AudioClip GetRandomClip() 
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
