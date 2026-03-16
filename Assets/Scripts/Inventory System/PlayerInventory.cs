using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;

    private void Update()
    {
//inventory.displayAllItemNames();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InstanceItemContainer foundItem))
            inventory.AddItem(foundItem.PickupItem());
    }

    public void equipWeapon(int partyIndex, int inventoryIndex)
    {
        inventory.equipWeapon(partyIndex, inventoryIndex);
    }
    public void equipArmor(int partyIndex, int inventoryIndex)
    {
        inventory.equipArmor(partyIndex, inventoryIndex);
    }
    public void equipTest()
    {
        equipWeapon(0, 0);
    }
}
