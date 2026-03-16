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
    //public TextMeshProUGUI[] inventoryLabels = new TextMeshProUGUI[15];
    public List<ItemInMenuObj> itemsInMenu = new();
    public GameObject menuObj;
    BattleManager battleMan;
    public PartyMenuTest partyMenu;
    public ShopManager shopMan;

    public int selectedPartyMember;

    private void Start()
    {
        battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
    }
    private void Update()
    {
        if (battleMan.battleOpen && menuObj.activeSelf) closeMenu();
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
        Debug.Log(inventoryIndex);
        Debug.Log(partyIndex);
        if (items[inventoryIndex].itemType.myType == ItemData.itemType.armor)
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
        partyMenu.childArray[partyIndex].updateInfo();
    }
    public void equipWeapon(int partyIndex, int inventoryIndex)
    {
        Debug.Log(inventoryIndex);
        if (items[inventoryIndex].itemType.myType == ItemData.itemType.weapon)
        {
            ItemInstance tempWeapon = battleMan.party[partyIndex].weapon;
            battleMan.party[partyIndex].weapon = items[inventoryIndex];
            if (tempWeapon != new ItemInstance()) items[inventoryIndex] = tempWeapon;
            else items.RemoveAt(inventoryIndex);
            battleMan.party[partyIndex].equipStatChange();
            Debug.Log("Item Equipped!");
        }
        else
        {
            Debug.LogError("Item is not a weapon!");
        }
        partyMenu.childArray[partyIndex].updateInfo();
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
        shopMan.closeShop();
    }
    public void closeMenu()
    {
        menuObj.SetActive(false);
    }
    public void updateLabels()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType != null)
            {
                itemsInMenu[i].label.text = items[i].itemType.itemName;
                itemsInMenu[i].myType = (ItemInMenuObj.itemType)items[i].itemType.myType;
            }
            else
            {
                itemsInMenu[i].label.text = "";
                itemsInMenu[i].myType = ItemInMenuObj.itemType.none;
            }
        }
    }
    public void triggerEquipFor(bool armor)
    {
        if (armor)
        {
            //Set buttons for armor list items to be visible
            for (int i = 0; i < items.Count; i++)
            {
                ItemInMenuObj item = itemsInMenu[i];
                if (item.myType == ItemInMenuObj.itemType.armor)
                {
                    int a = i;
                    Debug.Log(item.itemName + " is an armor");
                    item.equipButton.gameObject.SetActive(true);
                    item.equipButton.onClick.AddListener(() => equipArmor(selectedPartyMember, a));
                    item.equipButton.onClick.AddListener(() => hideAllButtons());
                }
            }
        }
        else
        {
            //Set buttons for weapon list items to be visible
            for(int i = 0; i < items.Count; i++)
            {
                ItemInMenuObj item = itemsInMenu[i];
                if (item.myType == ItemInMenuObj.itemType.weapon)
                {
                    int a = i;
                    Debug.Log(item.itemName + " is a weapon");
                    item.equipButton.gameObject.SetActive(true);
                    item.equipButton.onClick.AddListener(() => equipWeapon(selectedPartyMember, a));
                    item.equipButton.onClick.AddListener(() => hideAllButtons());
                }
            }

        }
    }
    public void setPlayer(PartyListObj player)
    {
        selectedPartyMember = player.indexInList;
    }
    public void hideAllButtons()
    {
        foreach (ItemInMenuObj item in itemsInMenu)
        {
            item.equipButton.gameObject.SetActive(false);
            item.equipButton.onClick.RemoveAllListeners();
        }
        updateLabels();
    }
}
