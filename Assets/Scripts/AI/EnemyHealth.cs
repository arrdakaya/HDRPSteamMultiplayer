using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
public class EnemyHealth : NetworkBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    private VisualEffect VFXGraph;
    private Animator anim;
    private SkinnedMeshRenderer skinnedMesh;
    private Material[] skinnedMaterials;
    private float dissolveRate = 0.0125f;
    private float refreshRate = 0.025f;

    [SyncVar][SerializeField] private float health;
    private float maxHealth = 100f;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        VFXGraph = transform.GetChild(2).GetComponent<VisualEffect>();
        anim = GetComponent<Animator>();
        if(skinnedMesh == null)
        {
            skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            skinnedMaterials = skinnedMesh.materials;
        }
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(int damage)
    {

        health -= damage;
        if(health <= 0)
        {
            Die();
        }
        
      
    }
    public void Die()
    {
        gameObject.GetComponent<ParasiteScript>().enabled = false;
        anim.SetTrigger("Die");
        StartCoroutine(DissolveEffect());
    }

    IEnumerator DissolveEffect()
    {
        agent.isStopped = true;
        if (VFXGraph != null)
        {
            VFXGraph.Play();
        }
        if(skinnedMaterials.Length > 0)
        {
            float counter = 0;
            while (skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
               

            }
        }
    }
}
