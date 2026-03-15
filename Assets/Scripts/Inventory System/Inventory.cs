using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxItems = 15;
    public List<ItemInstance> items = new();
    public TextMeshProUGUI[] inventoryLabels = new TextMeshProUGUI[15];
    public GameObject menuObj;
    BattleManager battleMan;

    private void Start()
    {
        battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }
    public void displayAllItemNames()
    {
        foreach (var item in items)
        {
            if(item.itemType != null)Debug.Log(item.itemType.name);
        }
    }
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

    public void TrashItem(int inventoryIndex)
    {
        items.RemoveAt(inventoryIndex);
    }

    public void equipArmor(int partyIndex, int inventoryIndex)
    {
        if (items[inventoryIndex].itemType.myType == ItemData.itemType.armor && items[inventoryIndex] != null)
        {
            ItemInstance tempArmor = battleMan.party[partyIndex].armor;
            battleMan.party[partyIndex].armor = items[inventoryIndex];
            if (tempArmor != null) items[inventoryIndex] = tempArmor;
            else items.RemoveAt(inventoryIndex);
            battleMan.party[partyIndex].equipStatChange();
            Debug.Log("Item Equipped!");
        }
        else
        {
            Debug.LogError("Item is not an armor!");
        }
    }
    public void equipWeapon(int partyIndex, int inventoryIndex)
    {
        if (items[inventoryIndex].itemType.myType == ItemData.itemType.weapon && items[inventoryIndex] != null)
        {
            ItemInstance tempWeapon = battleMan.party[partyIndex].armor;
            battleMan.party[partyIndex].weapon = items[inventoryIndex];
            if (tempWeapon != null) items[inventoryIndex] = tempWeapon;
            else items.RemoveAt(inventoryIndex);
            battleMan.party[partyIndex].equipStatChange();
            Debug.Log("Item Equipped!");
        }
        else
        {
            Debug.LogError("Item is not a weapon!");
        }
    }
    public void menuButton()
    {
        if (menuObj.activeSelf)
        {
            closeMenu();
        }
        else
        {
            openMenu();
        }
    }
    public void openMenu()
    {
        updateLabels();
        menuObj.SetActive(true);
    }
    public void closeMenu()
    {
        menuObj.SetActive(false);
    }
    public void updateLabels()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType != null) inventoryLabels[i].text = items[i].itemType.itemName;
            else inventoryLabels[i].text = "";
        }
    }
}
