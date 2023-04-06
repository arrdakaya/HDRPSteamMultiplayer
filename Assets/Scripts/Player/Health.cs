using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public static Health Instance;

    [SyncVar][SerializeField] private float health;
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private Image bloodSplatter;

    public Image frontHealthBar;
    public Image backHealthBar;
    private float chipSpeed = 2f;
    private float lerpTimer;

    private void Start()
    {
        health = maxHealth;
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealtUI();
            if (Input.GetKeyDown(KeyCode.H) && health != 100)
            {
                RestoreHealth(30);
            }
        }
    
    }
    public void DecreaseHP(int damage)
    {
        if (!isLocalPlayer) return;
        
        health -= damage;
        lerpTimer = 0f;
        StartCoroutine("ShowBlood");
       
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
       
    }
    IEnumerator ShowBlood()
    {
        bloodSplatter.gameObject.SetActive(true);
        Color bloodSplatterColor = bloodSplatter.color;
        bloodSplatterColor.a = 1;
        bloodSplatter.color = bloodSplatterColor;
        yield return new WaitForSeconds(1);
        bloodSplatterColor.a = 0;
        bloodSplatter.color = bloodSplatterColor;
        bloodSplatter.gameObject.SetActive(false);

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
