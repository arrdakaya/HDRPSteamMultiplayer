using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public static Health Instance;

    [SyncVar][SerializeField] private float health;
    [SerializeField] private float maxHealth = 100f;

    [Header("Health Bar")]
    public Image frontHealthBar;
    public Image backHealthBar;
    private float chipSpeed = 2f;
    private float lerpTimer;

    [Header("Damage Overlay")]
    [SerializeField] private Image overlay;
    [SerializeField] private float duration;
    [SerializeField] private float fadeSpeed;
    private float durationTimer;


    private void Start()
    {
        //health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealtUI();
            if (Input.GetKeyDown(KeyCode.H) && health != 100)
            {
                if (PlayerAbilities.getMoreHP)
                {
                    RestoreHealth(50);
                }
                else
                {
                    RestoreHealth(30);
                }
            }
            if(overlay.color.a > 0)
            {
                if (health < 20)
                    return;
                durationTimer += Time.deltaTime;
                if(durationTimer > duration)
                {
                    float tempAlpha = overlay.color.a;
                    tempAlpha -= Time.deltaTime * fadeSpeed;
                    overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);

                }
            }
        }
    
    }
    public void DecreaseHP(int damage)
    {
        if (!isLocalPlayer) return;
        
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        if(!isLocalPlayer) return;
        if(SaveScript.itemAmts[1] > 0)
        {
            health += healAmount;
            lerpTimer = 0f;
            SaveScript.itemAmts[1]--;
        }
        if (PlayerAbilities.increaseHP)
        {
            health += healAmount;
            lerpTimer = 0f;
        }
       
    }
 
    void UpdateHealtUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if(fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if(fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }
}
