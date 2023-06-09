using Mirror;

using UnityEngine;

public class AtmosphericMusic : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;

  
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
       

    }

    private void Update()
    {
        if(!audioSource.isPlaying)
        {
            AudioClip clip = GetRandomClip();
            audioSource.PlayOneShot(clip);
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    
                }


            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                AudioClip clip = GetRandomClip();
                audioSource.PlayOneShot(clip);
            }
        }
    }
    private AudioClip GetRandomClip()
    {
        return audioClips[Random.Range(0, audioClips.Length - 1)];
    }
}
