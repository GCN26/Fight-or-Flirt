using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    public BattleManager BattleManager;

    public void startEncounter(int index, string name)
    {
        //Index is for enemy table for boss
        BattleManager.enemyTableIndex = index;
        BattleManager.startBattle();
        BattleManager.startBattleBoss(name);
    }
    public void rocky()
    {
        startEncounter(1, "Rocky");
    }
}
