using System;
using UnityEngine;

public class LevelTransTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public SaveLoadSystem save;
    public LevelTransFade fade;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null)
        {
            save.saveGame();
            fade.loadAfter = true;
            fade.fadeOut();
            this.gameObject.SetActive(false);
        }
    }
}
