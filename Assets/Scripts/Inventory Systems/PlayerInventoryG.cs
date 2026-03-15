using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public BattleManager BattleManager;
    public item[] inventory = new item[15];
    public TextMeshProUGUI[] inventoryLabels = new TextMeshProUGUI[15];
    public GameObject menuObj;

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
        for (int i = 0; i < inventory.Length; i++)
        {
            inventoryLabels[i].text = inventory[i].name;
        }
    }
    public void testInventory()
    {
        inventory[0] = itemTables.armorTable[0];
        Debug.Log(((armor)inventory[0]).defense);
        inventory[1] = itemTables.weaponTable[0];
        Debug.Log(((weapon)inventory[1]).attack);
    }

    public void equipArmor(int partyIndex, int inventoryIndex)
    {
        if (inventory[inventoryIndex] is armor && inventory[inventoryIndex].myType == item.itemType.armor && inventory[inventoryIndex] != null)
        {
            armor tempArmor = BattleManager.party[partyIndex].armor;
            BattleManager.party[partyIndex].armor = (armor)inventory[inventoryIndex];
            if(tempArmor != null) inventory[inventoryIndex] = tempArmor;
            else inventory[inventoryIndex] = null;
            BattleManager.party[partyIndex].equipStatChange();
            Debug.Log("Item Equipped!");
        }
        else
        {
            Debug.LogError("Item is not an armor!");
        }
    }
    public void equipWeapon(int partyIndex, int inventoryIndex)
    {
        if (inventory[inventoryIndex] is weapon && inventory[inventoryIndex].myType == item.itemType.weapon && inventory[inventoryIndex] != null)
        {
            armor tempWeapon = BattleManager.party[partyIndex].armor;
            BattleManager.party[partyIndex].weapon = (weapon)inventory[inventoryIndex];
            if (tempWeapon != null) inventory[inventoryIndex] = tempWeapon;
            else inventory[inventoryIndex] = null;
            BattleManager.party[partyIndex].equipStatChange();
            Debug.Log("Item Equipped!");
        }
        else
        {
            Debug.LogError("Item is not a weapon!");
        }
    }
    public void testEquip()
    {
        equipArmor(0, 0);
    }
    public void testEquip2()
    {
        equipWeapon(0, 1);
    }
}

[Serializable]
public class item
{
    public string name;
    public string description;
    public int buyPrice, sellPrice;

    public enum itemType
    {
        none,
        armor,
        weapon,
        heal,
        item
    }
    public itemType myType = itemType.item;

    public item(string name = "Item", string desc = "An item.", int buy = 0, int sell = 0)
    {
        this.name = name;
        this.description = desc;
        this.buyPrice = buy;
        this.sellPrice = sell;
    }
    public virtual void nameAndType()
    {
        Debug.Log(name + " Item");
    }
}

[Serializable]
public class armor : item
{
    public int defense;
    public int speed;
    public int looks;
    public int intelligence;
    public int charisma;
    public int health;

    public armor(string name = "Armor", string desc = "An armor.", int buy = 0, int sell = 0, int def = 0, int spd = 0, int looks = 0, int intel = 0, int charisma = 0, int hp = 0) : base(name,desc,buy,sell)
    {
        this.defense = def;
        this.speed = spd;
        this.looks = looks;
        this.intelligence = intel;
        this.charisma = charisma;
        this.health = hp;
        myType = itemType.armor;
    }
    public override void nameAndType()
    {
        Debug.Log(name + " Armor");
        Debug.Log(defense);
    }
}

[Serializable]
public class weapon : item
{
    public int attack;
    public int speed;
    public int looks;
    public int intelligence;
    public int charisma;
    public int health;
    public weapon(string name = "Weapon", string desc = "A weapon.", int buy = 0, int sell = 0, int atk = 0, int spd = 0, int looks = 0, int intel = 0, int charisma = 0, int hp = 0) : base(name, desc, buy, sell)
    {
        this.attack = atk;
        this.speed = spd;
        this.looks = looks;
        this.intelligence = intel;
        this.charisma = charisma;
        this.health = hp;
        myType = itemType.weapon;
    }
    public override void nameAndType()
    {
        Debug.Log(name + " Weapon");
    }
}
[Serializable]
public class heal : item
{
    public int health;
    //Additional effects
    public heal(string name = "Item", string desc = "An item.", int buy = 0, int sell = 0, int hp = 0) : base(name, desc, buy, sell)
    {
        this.health = hp;
        myType = itemType.heal;
    }
    public override void nameAndType()
    {
        Debug.Log(name + " Heal");
    }
}

public static class itemTables
{
    public static item[] itemTable =
    {
        new item("Test Item", "A test item.")
    };
    public static armor[] armorTable =
    {
        new armor("Test Armor", "A test armor.", 1, 1, def: 5),
        new armor("Test Armor 2", "A test armor.", 1, 1, def: 4),
        new armor("Test Armor 3", "A test armor.", 1, 1, def: 3)
    };
    public static weapon[] weaponTable =
    {
        new weapon("Test Weapon", "A test weapon.", 1, 1, atk: 5)
    };
    public static heal[] healTable =
    {
        new heal("Test Heal", "A test healing item.", 1, 1, 15)
    };
}