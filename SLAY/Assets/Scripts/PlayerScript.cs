using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving = false;
    public Storage inventory;



    // Start is called before the first frame update
    void Awake()
    {
        inventory = new Storage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测是否发生了点击事件
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 获取点击位置的世界坐标
            targetPosition.z = 0; // 保证z轴为0（2D场景）
            isMoving = true; // 开始移动
        }

        Moving();
    }

    private void Moving()
    {
        if (isMoving)
        {
            float step = 3f * Time.deltaTime; // 移动速度
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step); // 移动至目标位置

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f) // 到达目标位置
            {
                isMoving = false; // 停止移动
            }
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
