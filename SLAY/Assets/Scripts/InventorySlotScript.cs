using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image imageComponent;
    public TextMeshProUGUI quantity;
    public virtual Item item { get; set; }

    // 每次物品格子修改时调用以下函数
    public void UpdateSlot()
    {
        var itemImage = imageComponent;

        // 如果物品为空或数量为0，则认为槽位为空
        if (item != null && item.quantity != 0)
        {
            // 槽位有物品：启用图标
            itemImage.enabled = true;
            itemImage.sprite = item.type.icon;

            // 如果槽位上的数量等于一，则无需启用数量UI文本
            if (item.quantity > 1)
            {
                quantity.enabled = true;
                quantity.text = item.quantity.ToString();
            }
            else
            {
                quantity.enabled = false;
            }

        }
        else
        {
            // 槽位空：禁用图标、数量
            itemImage.enabled = false;
            quantity.enabled = false;
        }
    }

}

public class InventorySlotScript : ItemSlot
{
    public override Item item { get { return storage.GetItemByIndex(slotIndex); } }
    public Storage storage;
    public int slotIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
