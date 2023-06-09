using Mirror;

using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{
    // Start is called before the first frame update
    public static EnemyHealth Instance;

    [SyncVar][SerializeField] private float health;
    private float maxHealth = 100f;

    private void Start()
    {
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
        Debug.Log("Enemy Dead");
    }
}
