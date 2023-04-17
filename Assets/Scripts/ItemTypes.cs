using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTypes : MonoBehaviour
{
    public enum typeOfItem
    {
        battery,
        medkit,
        key,
        crown
    }
    public typeOfItem chooseItem;
}
