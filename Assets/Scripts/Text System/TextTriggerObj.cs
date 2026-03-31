using System;
using UnityEngine;

public class TextTriggerObj : MonoBehaviour
{
    public TextEventManager textMan;
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null)
        {
            textMan.callText(index);
            Destroy(this.gameObject);
        }
    }
}
