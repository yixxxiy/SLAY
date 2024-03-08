using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XGame;

public class PlayerScript : MonoSingleton<PlayerScript>
{
    const float HUNGER_DECREASE_INTERVAL = 5f;

    public Storage inventory;
    public float speed = 3.0f;
    public int health;
    public int maxHealth = 100;
    public int hunger;
    public int maxHunger = 100;
    public int exp = 0;
    public int maxExp = 100;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float hungerTimer = 0f;
    MeleeAttack meleeAttack;

    void Awake()
    {
        inventory = new Storage(40);
        health = maxHealth;
        hunger = maxHunger;

        this.RegisterEvent<PlayerInteractEvent>(PlayerInteract);
        meleeAttack = GetComponent<MeleeAttack>();
    }

    void PlayerInteract(PlayerInteractEvent e)
    {
        Debug.Log("got PlayerInteractEvent");
        switch (e.type)
        {
            case InteractType.Attack:
                meleeAttack.Use();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMoving();
        HandleHunger();
    }

    void HandleHunger()
    {
        hungerTimer += Time.deltaTime;
        int hungerDecreaseUnit = (int)Math.Floor(hungerTimer / HUNGER_DECREASE_INTERVAL);
        if (hungerDecreaseUnit <= 0) return;
        hungerTimer -= (float)hungerDecreaseUnit * HUNGER_DECREASE_INTERVAL;
        int hungerDecrease = hungerDecreaseUnit * 10;
        Debug.Log(string.Format("player's hunger -{0}", hungerDecrease));
        hunger -= hungerDecrease;
        if (hunger < 0)
        {
            hunger = 0;
        }
    }

    private void HandleMoving()
    {
        var joystick = JoystickSingleton.instance;
        if (joystick == null) return;

        var x = joystick.Horizontal;
        var y = joystick.Vertical;
        isMoving = new Vector2(x, y).magnitude >= 0.1f;
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            transform.position += step * new Vector3(x, y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "掉落物":
                DropItemScript dropItemScript = collision.gameObject.GetComponent<DropItemScript>();
                Item remainItem = inventory.AddItem(dropItemScript.item);
                if (remainItem == null)
                {
                    collision.SendMessage("Pick");
                }


                //
                //if(!XGame.MainController.NeedGamePause())
                //{

                //}
                //AudioManager.Instance.PlayOneShot(AudioModel.Cloect);
                //AudioManager.Instance.StopAudio(AudioModel.Cloect);
                //EventCenterManager.Send<SentUpdateGoldText>(new SentUpdateGoldText() { isShowPalu = true });  //发送
                //XGame.MainController.DataMgr.AddReward(Reward.Gold,50);



                //XGame.MainController.ShowUI<UI_Menu>(new MenuDate()
                //{
                //    PALUID = 1,

                //    dongtaiwuti = this.gameObject,
                //}) ;
                //XGame.MainController.HideUI<UI_Menu>();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        this.UnRegisterEvent<PlayerInteractEvent>();
    }
}
