using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBarScript : StaticInventoryScript
{
    public GameObject player;

    void Start()
    {
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        SetStorage(playerScript.inventory);
    }
}
