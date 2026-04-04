using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    public BattleManager BattleManager;
    public TextEventManager TextEventManager;

    public void startEncounter(int index, string name)
    {
        //Index is for enemy table for boss
        BattleManager.enemyTableIndex = index;
        BattleManager.startBattle();
    }
    public void rocky()
    {
        startEncounter(1, "Rocky");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMovementScript>() != null)
        {
            this.TextEventManager.callText(104);
            Destroy(this.gameObject);
        }
    }
}
