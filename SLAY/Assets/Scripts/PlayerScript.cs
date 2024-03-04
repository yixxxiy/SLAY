using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving = false;
    public Storage inventory;
    public Joystick joystick;
    public float speed = 3.0f;

    // Start is called before the first frame update
    void Awake()
    {
        inventory = new Storage();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMoving();
    }

    private void HandleMoving()
    {
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
                inventory.AddItem(dropItemScript.item);

                collision.SendMessage("Pick");
                break;
            default:
                break;
        }
    }
}
