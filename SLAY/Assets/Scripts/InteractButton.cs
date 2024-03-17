using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XGame;

public class InteractButton : MonoBehaviour
{
    InteractType interactType = InteractType.Attack;
    Button btn;

    private void Awake()
    {
        this.RegisterEvent<UpdateInteractEvent>(UpdateInteract);

        btn = GetComponent<Button>();
        btn.AddClickEvent(Interact);
    }

    void UpdateInteract(UpdateInteractEvent e)
    {
        Debug.Log("got UpdateInteractEvent");
        interactType = e.type;
        //TODO: update UI
    }

    void Interact()
    {
        Debug.Log("player interact");
        EventCenterManager.Send(new PlayerInteractEvent
        {
            type = interactType,
        });
    }

    private void OnDestroy()
    {
        this.UnRegisterEvent<UpdateInteractEvent>();
    }
}
