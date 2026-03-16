using System;
using UnityEngine;

public class ShopCol : MonoBehaviour
{
    public ShopManager shopMan;
    bool active = true;
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("COllision");
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null && active)
        {
            shopMan.openShop();
            active = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null && !active)
        {
            active = true;
        }
    }
}
