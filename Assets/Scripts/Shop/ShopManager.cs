using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameManager gameMan;
    public PlayerInventory inventory;

    public shopItem[] stock;

    void Start()
    {
        foreach (shopItem item in stock)
        {
            item.shopMan = this;
            item.getVars();
        }
    }
    public void buyItem(int index)
    {
        if (stock[index].buyable)
        {
            int loopIndex = -1;
            for (int i = 0; i < inventory.inventory.Length; i++)
            {
                if (inventory.inventory[i].name == "")
                {
                    loopIndex = i;
                    i = inventory.inventory.Length;
                }
            }
            if (loopIndex == -1)
            {
                //Inventory Full
                Debug.Log("Inventory Full");
            }
            else
            {
                if (gameMan.money >= stock[index].cost)
                {
                    //Purchase successful
                    Debug.Log("Purchase Successful");
                    gameMan.money -= stock[index].cost;
                    inventory.inventory[loopIndex] = stock[index].itemSelf;
                    stock[index] = null;
                }
                else
                {
                    //Broke :(
                    Debug.Log("Insufficent Funds");
                }
            }
        }
    }
}

[Serializable]
public class shopItem
{
    public ShopManager shopMan;
    public item.itemType type;
    public int itemID;

    public item itemSelf;
    public int cost;
    public string name;
    public string description;
    public bool buyable = false;

    public void getVars()
    {
        item theItem = null;
        switch (type)
        {
            case item.itemType.none: break;
            case item.itemType.armor: theItem = itemTables.armorTable[itemID]; break;
            case item.itemType.weapon: theItem = itemTables.weaponTable[itemID]; break;
            case item.itemType.heal: theItem = itemTables.healTable[itemID]; break;
            case item.itemType.item: theItem = itemTables.itemTable[itemID];break;
        }
        buyable = true;
        itemSelf = theItem;
        description = theItem.description;
        name = theItem.name;
    }
}
