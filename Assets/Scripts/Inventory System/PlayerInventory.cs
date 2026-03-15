using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out InstanceItemContainer foundItem))
            inventory.AddItem(foundItem.PickupItem());
    }
}
