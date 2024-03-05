using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public struct ItemType
{
    public string itemName;
    public string description;
    public int maxStack; //最大堆叠数量
    public Sprite icon; //物品图标
    public List<string> tags; //物品标签

    //public static string defaultPath = "Items";

    public bool stackable
    {
        get
        {
            return maxStack > 1;
        }
    }

    public ItemType(string name, string desc = "", int stack = 1, string iconPath = "", int spriteIndex = 0)
    {
        this.itemName = name;
        this.description = desc;
        this.maxStack = stack;
        //if (iconPath == "")
        //{
        //    iconPath = Path.Combine(defaultPath, name);
        //}
        //this.icon = Resources.Load<Sprite>(iconPath);
        Sprite[] icons = Resources.LoadAll<Sprite>(iconPath);
        this.icon = icons[spriteIndex];
        this.tags = new List<string>();
    }
}

public static class ItemTypes
{
    public static Dictionary<string, ItemType> itemTypes;
    private static string ITEM_PATH = "Untitled Artwork";

    public static void Init()
    {

        itemTypes = new Dictionary<string, ItemType>();
        //itemTypes.Add("", new ItemType("", "", 1, ITEM_PATH, 0));
        itemTypes.Add("木头", new ItemType("木头", "", 100, ITEM_PATH, 2));
        itemTypes.Add("石头", new ItemType("石头", "", 100, ITEM_PATH, 1));
    }
}

[Serializable]
public class Item
{
    public string name;
    public string data;
    public int quantity;

    public ItemType type
    {
        get
        {
            return ItemTypes.itemTypes[name];
        }
    }

    // 设置物品为空
    public void SetEmpty()
    {
        this.name = "";
        this.data = "";
        this.quantity = 0;
    }

    public bool isEmpty
    {
        get
        {
            return string.IsNullOrEmpty(name) || (quantity <= 0);
        }
    }

    public Item(string name = "", string data = "", int quantity = 1)
    {
        this.name = name;
        this.data = data;
        this.quantity = quantity;
    }

    public Item(Item original)
    {
        name = original.name;
        data = original.data;
        quantity = original.quantity;
    }
}

[Serializable]
public class Slot
{
    public Item item;

    public Slot(Item item = null)
    {
        if (item == null)
        {
            item = new Item();
        }
        this.item = item;
    }

    // 获得格子的剩余空间
    public int GetRemainSpace(string name = "")
    {
        if (name == "")
        {
            name = item.name;
        }

        if (item.isEmpty)
        {
            return ItemTypes.itemTypes[name].maxStack;
        }
        else
        {
            if (name == item.name)
            {
                return item.type.maxStack - item.quantity;
            }
            else
            {
                return 0;
            }
        }
    }

    public void AddItem(Item item)
    {
        if (this.item.isEmpty)
        {
            this.item = item;
            return;
        }

        this.item.quantity += item.quantity;
        this.item.data += item.data; //暂时处理数据的方式
        if (this.item.quantity > this.item.type.maxStack)
        {
            Debug.LogWarning($"错误：添加物品{item.name}后，物品数量{this.item.quantity}超过最大堆叠数量{item.type.maxStack}！");
        }
    }

    public Item GetAll()
    {
        Item getItem = item;
        item = new Item();
        return getItem;
    }

    // 从格子中拿走物品
    public Item GetItem(int maxAmount)
    {
        if (item.quantity <= maxAmount)
        {
            return GetAll();
        }
        else
        {
            item.quantity -= maxAmount;
            return new Item(item.name, item.data, maxAmount);
        }
    }

    public void Increase(int amount)
    {
        item.quantity += amount;
        if (item.quantity > item.type.maxStack)
        {
            Debug.LogWarning($"错误：添加物品{item.name}后，物品数量{this.item.quantity}超过最大堆叠数量{item.type.maxStack}！");
        }  
    }

    public void Remove(int amount)
    {
        item.quantity -= amount;
        if (item.quantity < 0)
        {
            Debug.LogWarning($"错误：移除物品后，物品数量{item.quantity}为负！");
        }

        if (item.quantity <= 0)
        {
            item.SetEmpty();
        }
    }
}

[Serializable]
public class Storage
{
    public List<Slot> slotList;

    public Storage(int slotCount)
    {
        slotList = new List<Slot>();
        for (int i = 0; i < slotCount; i++)
        {
            slotList.Add(new Slot());
        }
    }

    // 获取指定物品的剩余空间
    public int GetRemainSpace(string name)
    {
        int remainSpace = 0;
        foreach (var slot in slotList)
        {
            remainSpace += slot.GetRemainSpace(name);
        }
        return remainSpace;
    }

    private void AddItemSimple(Item item)
    {
        foreach (var slot in slotList)
        {
            int space = slot.GetRemainSpace(item.name);
            if (space >= item.quantity)
            {
                slot.AddItem(item);
                item = null;
                break;
            }
            else
            {
                item.quantity -= space;
                slot.Increase(space);
            }
        }
        if (item != null)
        {
            Debug.LogWarning($"错误：仓库添加物品{item.name}时出现溢出，溢出数量：{item.quantity}");
        }
    }

    // 向仓库添加物品，返回剩余物品，如果没有剩余返回null
    public Item AddItem(Item item)
    {
        int remainSpace = GetRemainSpace(item.name);
        if (remainSpace >= item.quantity)
        {
            AddItemSimple(item);
            return null;
        }
        else
        {
            item.quantity -= remainSpace;
            Item itemToAdd = new Item(item);
            itemToAdd.quantity = remainSpace;
            AddItemSimple(itemToAdd);
            return item;
        }

        //每次添加物品时更新库存UI
        //UpdateInventoryUI();
    }

    public void RemoveItem(Item item)
    {
        //itemList.Remove(item);
    }

    //public string ItemsToString()
    //{
    //    StringBuilder stringBuilder = new StringBuilder();
    //    foreach (var item in itemList)
    //    {
    //        stringBuilder.AppendLine($"Name: {item.name}, Data: {item.data}, Quantity: {item.quantity}");
    //    }
    //    return stringBuilder.ToString();
    //}

    public Item GetItemByIndex(int index)
    {
        if (index >= 0 && index < slotList.Count)
        {
            return slotList[index].GetAll();
        }
        else
        {
            return null;
        }
    }

    public Slot GetSlotByIndex(int index)
    {
        if (index >= 0 && index < slotList.Count)
        {
            return slotList[index];
        }
        else
        {
            return null;
        }
    }
}
