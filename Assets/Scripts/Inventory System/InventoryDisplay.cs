using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public Inventory inventory;
    public ItemDisplay[] slots;
    void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].UpdateItemDisplay(inventory.items[i].itemType.icon, i);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void dropItem(int index)
    {
        GameObject droppedItem = new GameObject();
        droppedItem.AddComponent<Rigidbody>();
        droppedItem.AddComponent<InstanceItemContainer>().item = inventory.items[index];
        GameObject itemModel = Instantiate(inventory.items[index].itemType.model, droppedItem.transform);

        inventory.items.RemoveAt(index);

        UpdateInventory();
    }
}
