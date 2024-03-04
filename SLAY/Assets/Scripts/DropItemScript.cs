using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemScript : MonoBehaviour
{
    public Item item;
    private SpriteRenderer spriteRenderer;
    //public Sprite defaultSprite; // 默认精灵

    private bool isNeedUpdate = false; //是否需要刷新

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent <SpriteRenderer>();
        //spriteRenderer.sprite = defaultSprite; // 在对象实例化时设置默认精灵
    }

    // Update is called once per frame
    void Update()
    {
        if (isNeedUpdate)
        {
            spriteRenderer.sprite = item.type.icon;
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        isNeedUpdate = true;
    }

    public void Pick()
    {
        Destroy(gameObject);
    }
}
