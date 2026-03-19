using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyListObj : MonoBehaviour
{
    public GameObject listObj;
    public Button upButton,downButton;
    public PartyMenuTest menu;

    public TextMeshProUGUI charName;
    public TextMeshProUGUI weaponName, armorName;
    public TextMeshProUGUI stats;
    public Slider hpBar;
    public Button changeEquipButton;

    public int indexInList;

    private void Start()
    {
        updateInfo();
    }
    private void Awake()
    {
        updateInfo();
    }
    public void updateInfo()
    {
        int index = Array.IndexOf(menu.childArray, this);
        menu.battleManager.party[index].equipStatChange();
        charName.text = menu.battleManager.party[index].charName;
        if (menu.battleManager.party[index].weapon.itemType != null) weaponName.text = menu.battleManager.party[index].weapon.itemType.itemName;
        else weaponName.text = "";
        if (menu.battleManager.party[index].armor.itemType != null) armorName.text = menu.battleManager.party[index].armor.itemType.itemName;
        else armorName.text = "";
        stats.text = (menu.battleManager.party[index].attack).ToString() + "\n" + (menu.battleManager.party[index].defense).ToString() + "\n" + menu.battleManager.party[index].speed.ToString() + "\n" + menu.battleManager.party[index].charisma.ToString();
        hpBar.value = (float)menu.battleManager.party[index].hp / (float)menu.battleManager.party[index].maxHp;
    }

    public void pressUp()
    {
        if (Array.IndexOf(menu.childArray,this) > 1)
        {
            int index = Array.IndexOf(menu.childArray, this);
            PartyListObj temp = menu.childArray[index - 1];
            menu.childArray[index - 1] = this;
            menu.childArray[index] = temp;

            Combatant tempC = menu.battleManager.party[index - 1];
            menu.battleManager.party[index - 1] = menu.battleManager.party[index];
            menu.battleManager.party[index] = tempC;
        }
        menu.updateOrder();
        updateInfo();
    }
    public void pressDown()
    {
        if (Array.IndexOf(menu.childArray, this) < menu.childArray.Length-1)
        {
            int index = Array.IndexOf(menu.childArray, this);
            PartyListObj temp = menu.childArray[index + 1];
            menu.childArray[index + 1] = this;
            menu.childArray[index] = temp;

            Combatant tempC = menu.battleManager.party[index + 1];
            menu.battleManager.party[index + 1] = menu.battleManager.party[index];
            menu.battleManager.party[index] = tempC;
        }
        menu.updateOrder();
        updateInfo();
    }
}
