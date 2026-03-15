using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public int maxItems = 15;
    public List<ItemInstance> items = new();


    public bool AddItem(ItemInstance addedItem)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = addedItem;
                return true;
            }
        }

        if (items.Count < maxItems)
        {
            items.Add(addedItem);
            return true;
        }

        Debug.Log("Full Inventory!");
        return false;
    }

    public void TrashItem(ItemInstance destroyItem)
    {
        items.Remove(destroyItem);
    }

    
}
