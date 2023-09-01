using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{

    public static bool lessDamageParasite = false;
    public static bool lessDamageTrap = false;
    public static bool bamboozled = false;
    public static bool getMoreHP = false;
    public static bool increaseHP = false;
    public static bool speedUp = false;
    public static bool giveMoreDamage = false;

   public bool GetLessDamageTrap()
    {
        return lessDamageTrap;
    }

}
