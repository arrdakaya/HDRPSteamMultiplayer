using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private GameObject localPlayer;
    public GameObject abilityUpgradeMenu;
    public List<Button> abilityCards = new List<Button>();
    public List<Button> activeAbilityCards = new List<Button>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;
    private void Start()
    {
        localPlayer = GameObject.Find("LocalGamePlayer");
       

    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            abilityUpgradeMenu.SetActive(true);
        }
        if(abilityUpgradeMenu.activeSelf == true)
        {
            DrawCard();
            localPlayer.GetComponent<PlayerMovementController>().canCameraMove = false;
            localPlayer.GetComponent<PlayerMovementController>().canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void DrawCard()
    {
        for(int i =0; i< availableCardSlots.Length; i++)
        {
            if(abilityCards.Count > 0)
            {
                Button randomCard = abilityCards[Random.Range(0, abilityCards.Count)];

                if (availableCardSlots[i] == true)
                {
                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = cardSlots[i].position;
                    availableCardSlots[i] = false;
                    abilityCards.Remove(randomCard);
                    activeAbilityCards.Add(randomCard);
                }
            }
           
           
        }

    }
    public void GetMoreHP()
    {
        if (!PlayerAbilities.getMoreHP)
        {
            PlayerAbilities.getMoreHP = true;
            CardSelected();
        }
        
    }
    public void IncreaseHP()
    {
        PlayerAbilities.increaseHP = true;
        localPlayer.GetComponent<Health>().RestoreHealth(30);
        CardSelected();
        

    }
    public void LessDamageTrap()
    {
        if (!PlayerAbilities.lessDamageTrap)
        {
            PlayerAbilities.lessDamageTrap = true;
            CardSelected();
        }
    }
    public void SpeedUpAbility()
    {
        if (!PlayerAbilities.speedUp)
        {
            PlayerAbilities.speedUp = true;
            localPlayer.GetComponent<PlayerMovementController>().MoveSpeed *= 1.3f;
            localPlayer.GetComponent<PlayerMovementController>().SprintSpeed *= 1.3f;
            CardSelected();
        }
    }
    public void CardSelected()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        localPlayer.GetComponent<PlayerMovementController>().canCameraMove = true;
        localPlayer.GetComponent<PlayerMovementController>().canMove = true;
        for (int i = 0; i < 3; i++)
        {
            availableCardSlots[i] = true;
           
        }
        for(int i = 0; i < activeAbilityCards.Count; i++)
        {
            if (activeAbilityCards[i].gameObject.activeSelf == true)
            {
                activeAbilityCards[i].gameObject.SetActive(false);

            }
        }
        abilityUpgradeMenu.SetActive(false);
        Debug.Log("Kart Seçildi");

    }
}
