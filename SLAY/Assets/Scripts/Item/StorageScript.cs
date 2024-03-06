using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public Storage storage;
    public int startIndex = 0; // 从库存的第几个开始显示物品
    public List<InventorySlotScript> slotList = new List<InventorySlotScript>();
    //public int slotCount = 0;
    //public int 

    void Start()
    {

    }

    public virtual void SetStorage(Storage storage)
    {
        this.storage = storage;
    }

    public void Rebind()
    {
        slotList.Clear();
        int index = startIndex;
        foreach (InventorySlotScript child in gameObject.GetComponentsInChildren<InventorySlotScript>())
        {
            child.storage = storage;
            child.slotIndex = index;
            index++;
            slotList.Add(child);
        }
        UpdateInventoryUI();
    }

    void Update()
    {
        UpdateInventoryUI();
    }

    // Everytime an item is Added or Removed from the Inventory, the UpdateInventoryUI runs
    public void UpdateInventoryUI()
    {
        // For each slot in the list it's attributed an Item from the itemList and the corresponding quantity
        foreach (InventorySlotScript slot in slotList)
        {
            // Calls the UpdateSlot() function on the respective slot and attributes the item and quantity of their unique index in the itemList
            slot.UpdateSlot();
        }
    }
}

public class StaticInventoryScript : InventoryScript
{
    public override void SetStorage(Storage storage)
    {
        this.storage = storage;
        Rebind();
    }
}

public class DynamicInventoryScript : InventoryScript
{
    public GameObject slotPrefeb;

    public new void SetStorage(int slotCount)
    {
        ClearSlots();
        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefeb, transform);
        }
        Rebind();
    }

    private void ClearSlots()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform != transform) // 避免销毁自身
            {
                Destroy(childTransform.gameObject);
            }
        }
    }
}
