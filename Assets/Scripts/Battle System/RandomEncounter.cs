using System;
using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    public int[] indexes;
    public BattleManager battleManager;
    public bool bossEncounter;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Encounter at " + this.gameObject.name);
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null)
        {
            if (!bossEncounter)
            {
                battleManager.enemyTableIndex = indexes[UnityEngine.Random.Range(0, indexes.Length-1)];
                battleManager.startBattle();
                this.gameObject.SetActive(false);
            }
            else
            {
                battleManager.enemyTableIndex = 1;
                battleManager.startBattle();
                battleManager.startBattleBoss("Rocky");
                this.gameObject.SetActive(false);
            }
        }
    }
}
