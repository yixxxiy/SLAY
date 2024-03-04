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
    }
}

public static class ItemTypes
{
    public static Dictionary<string, ItemType> itemTypes;
    private static string ITEM_PATH = "Untitled Artwork";

    public static void Init()
    {

        itemTypes = new Dictionary<string, ItemType>();
        itemTypes.Add("木头", new ItemType("木头", "", 1, ITEM_PATH, 2));
        itemTypes.Add("石头", new ItemType("石头", "", 1, ITEM_PATH, 1));
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

    public bool isEmpty
    {
        get
        {
            return string.IsNullOrEmpty(name) && (quantity <= 0);
        }
    }

    public Item(string name = "", string data = "", int quantity = 1)
    {
        this.name = name;
        this.data = data;
        this.quantity = quantity;
    }
}

[Serializable]
public class Storage
{
    public List<Item> itemList;

    public Storage()
    {
        itemList = new List<Item>();
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        //如果物品是可堆叠的，则检查库存中是否已经有该物品，并且只增加数量
        if (item.type.stackable)
        {
            //if (itemList.Contains(item))
            //{
            //    quantityList[itemList.IndexOf(item)] = quantityList[itemList.IndexOf(item)] + quantityAdded;
            //}
            //else
            //{

            //    if (itemList.Count < slotList.Count)
            //    {
            //        itemList.Add(item);
            //        quantityList.Add(quantityAdded);
            //    }
            //    else { }

            //}

        }
        //else
        //{
        //    for (int i = 0; i < quantityAdded; i++)
        //    {
        //        if (itemList.Count < slotList.Count)
        //        {
        //            itemList.Add(item);
        //            quantityList.Add(1);
        //        }
        //        else { }

        //    }

        //}

        //每次添加物品时更新库存UI
        //UpdateInventoryUI();
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
    }

    public string DebugDisplayItems()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in itemList)
        {
            stringBuilder.AppendLine($"Name: {item.name}, Data: {item.data}, Quantity: {item.quantity}");
        }
        return stringBuilder.ToString();
    }

    public Item GetItemByIndex(int index)
    {
        if (index >= 0 && index < itemList.Count)
        {
            return itemList[index];
        }
        else
        {
            return null;
        }
    }
}
