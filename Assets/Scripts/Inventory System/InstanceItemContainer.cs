using UnityEngine;

public class InstanceItemContainer : MonoBehaviour
{
    public ItemInstance item;

    public ItemInstance PickupItem()
    {
        Destroy(gameObject);
        return item;
    }
}
