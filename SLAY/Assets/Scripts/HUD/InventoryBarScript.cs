using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGame;

public class InventoryBarScript : StaticInventoryScript
{
    void Start()
    {
        SetStorage(PlayerScript.Instance.inventory);
    }
}
