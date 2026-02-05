using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Combatant[] party;
    public Combatant[] enemies;

    int currentPartyIndex;

    public List<Combatant> battleList;
    public Coroutine battleCo;

    void Start()
    {
        
        //battleCo = StartCoroutine(battleProcess());
    }

    public void testFight()
    {
        //party[0].attackEnemy(enemies[0]);
        battleCo = StartCoroutine(battleProcess());
    }
    public void testFlirt()
    {
        //party[0].rizzEnemy(enemies[0]);
    }

    IEnumerator battleProcess()
    {
        int partyDeadInt = 0;
        foreach (Combatant comb in party) {
            if(comb.hp > 0)
            {
                
            }
            else
            {
                partyDeadInt++;
            }
        }
        if(party.Length == partyDeadInt)
        {
            Debug.Log("You lose!");
            //End in Loss
            StopCoroutine(battleCo);
        }

        int enemyDeadInt = 0;
        foreach (Combatant comb in enemies)
        {
            if (comb.hp > 0 && comb.infatuation > 0)
            {

            }
            else
            {
                enemyDeadInt++;
            }
        }
        if (enemies.Length == enemyDeadInt)
        {

            Debug.Log("You win!");
            //End in win
            StopCoroutine(battleCo);
        }

        //Get Player Input
        while (currentPartyIndex < party.Length)
        {
            if (party[currentPartyIndex].hp > 0)
            {
                battleList.Add(party[currentPartyIndex]);
                currentPartyIndex++;
            }
        }
        int enemyIndex = 0;
        while(enemyIndex < enemies.Length)
        {
            if (enemies[enemyIndex].hp > 0)
            {
                battleList.Add(enemies[enemyIndex]);
                enemyIndex++;
            }
        }

        //Get Enemy Turn order
        foreach (Combatant comb in battleList)
        {
            comb.attackEnemy(enemies[0]);
        }

        currentPartyIndex = 0;
        battleList.Clear();
        battleCo = StartCoroutine(battleProcess());
        return null;
    }

    int checkSlots(int num)
    {
        if (num < 0) return -1;
        if (party[num].hp > 0) return num;
        else
        {
            if (num > 0) return checkSlots(num - 1);
            else return -1;
        }
    }
}
