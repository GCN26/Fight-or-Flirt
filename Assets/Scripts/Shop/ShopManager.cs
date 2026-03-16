using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameManager gameMan;
    public Inventory inventory;

    public shopItem[] stock;
    public ShopitemObj itemObjectPrefab;
    public GameObject panel;
    public List<ShopitemObj> items;

    public TextMeshProUGUI moneycount;
    public GameObject window;

    void Start()
    {
        populateShop();
    }
    public void populateShop()
    {
        moneycount.text = gameMan.money.ToString();
        int loop = 0;
        foreach (shopItem item in stock)
        {
            int a = loop;
            item.shopMan = this;
            item.getVars();

            ShopitemObj itemObj = Instantiate(itemObjectPrefab);
            itemObj.shopMan = this;
            itemObj.nameTMP.text = item.name;
            itemObj.costTMP.text = item.cost.ToString();

            itemObj.buyButton.onClick.AddListener(() => buyItem(a));
            itemObj.transform.SetParent(panel.transform);
            items.Add(itemObj);
            loop++;
        }
    }
    public void openShop()
    {
        window.SetActive(true);
    }
    public void closeShop()
    {
        window.SetActive(false);
    }
    public void buyItem(int index)
    {
        if (stock[index].buyable)
        {
            int loopIndex = -1;
            for (int i = 0; i < inventory.items.Count; i++)
            {
                if (inventory.items[i].itemType == null)
                {
                    loopIndex = i;
                    i = inventory.items.Count;
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
                    inventory.items[loopIndex].itemType = stock[index].itemInstance.itemType;
                    stock[index] = null;
                    Destroy(items[index].gameObject);
                }
                else
                {
                    //Broke :(
                    Debug.Log("Insufficent Funds");
                }
            }
        }
        moneycount.text = gameMan.money.ToString();
        gameMan.inventoryman.updateLabels();
    }
}

[Serializable]
public class shopItem
{
    public ShopManager shopMan;
    public ItemInstance itemInstance;
    public int cost;
    public string name;
    public string description;
    public bool buyable = false;

    public void getVars()
    {
        buyable = true;
        description = itemInstance.itemType.description;
        name = itemInstance.itemType.name;
    }
}
