using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JeraldBagPickup : MonoBehaviour
{
    public Inventory inventory;
    public ItemInstance letter, flowers;
    public TextEventManager textguy;

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CharacterMovementScript>() != null)
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
                inventory.items[loopIndex].itemType = letter.itemType;
                inventory.items[loopIndex+1].itemType = flowers.itemType;
                textguy.callText(85);
                Destroy(this.gameObject);
            }
        }
    }
}
