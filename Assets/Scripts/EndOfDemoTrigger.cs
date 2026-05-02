using System;
using UnityEngine;

public class EndOfDemoTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public LevelTransFade fade;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null)
        {
            fade.goToEndOfDemo = true;
            fade.fadeOut();
            this.gameObject.SetActive(false);
        }
    }
}
