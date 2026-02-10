using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Combatant[] party;
    public List<Combatant> enemies;

    int currentPartyIndex;

    public List<Combatant> battleList;
    public Coroutine battleCo;

    public bool battleOpen;
    public GameObject battleUI;
    bool pausedForInput;

    public GameObject partyHPBox;
    public GameObject enemyHPBox;
    public Slider[] partyHPSliders;
    public Slider[] enemyHPSliders;

    public GameObject attackTargetPanel;
    public GameObject rizzTargetPanel;
    //Add item/party target panel
    //Change type of array to buttons?
    public GameObject[] attackTargetButtons;
    public GameObject[] rizzTargetButtons;

    public GameObject attackMovePanel;
    public GameObject rizzMovePanel;

    public MoveButton[] attackMoveButtons;
    public MoveButton[] rizzMoveButtons;
    void Start()
    {
        
        //battleCo = StartCoroutine(battleProcess());
    }

    public void fightButton(int index)
    {
        party[currentPartyIndex].target = enemies[index];
        pausedForInput = false;
    }
    public void flirtButton(int index)
    {
        party[currentPartyIndex].target = enemies[index];
        pausedForInput = false;
    }

    public void startBattle()
    {
        if (!battleOpen)
        {
            enemies.Add(new(enemyList.enemyTable[0]));
            enemies.Add(new(enemyList.enemyTable[1]));
            enemies.Add(new(enemyList.enemyTable[0]));
            enemies.Add(new(enemyList.enemyTable[1]));

            battleOpen = true;
            //Add way to customize encounters
            //Add encounter table for enemies
            foreach (Combatant partyMember in party)
            {
                partyMember.attackList.Clear();
                partyMember.getAttacksInList();
            }
            foreach (Combatant enemy in enemies)
            {
                enemy.attackList.Clear();
                enemy.getAttacksInList();
            }

            battleCo = StartCoroutine(battleProcess());
        }
    }
    public void endBattle()
    {
        disableHealthBars();
        battleList.Clear();
        enemies.Clear();
        battleOpen = false;
        battleUI.SetActive(false);
    }

    IEnumerator battleProcess()
    {
        enableHealthBars();
        pausedForInput = true;
        updateHealthBars();
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
            endBattle();
            //End in Loss
            //StopCoroutine(battleCo);
            yield break;
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
        if (enemies.Count == enemyDeadInt)
        {

            Debug.Log("You win!");
            endBattle();
            //End in win
            //StopCoroutine(battleCo);
            yield break;
        }

        //Get Player Input
        while (currentPartyIndex < party.Length)
        {
            if (party[currentPartyIndex].hp > 0)
            {
                party[currentPartyIndex].target = null;
                battleUI.SetActive(true);
                setMoveList(currentPartyIndex);

                while (pausedForInput) yield return null;

                closeAttackPanel();
                closeRizzPanel();
                battleList.Add(party[currentPartyIndex]);
                currentPartyIndex++;
                pausedForInput = true;
            }
            battleUI.SetActive(false);
            clearMoveList();
        }
        int enemyIndex = 0;
        while(enemyIndex < enemies.Count)
        {
            if (enemies[enemyIndex].hp > 0 && enemies[enemyIndex].infatuation > 0)
            {
                enemies[enemyIndex].target = party[checkSlots(UnityEngine.Random.Range(0, party.Length))];
                battleList.Add(enemies[enemyIndex]);
            }
            enemyIndex++;
        }

        //Get Enemy Turn order
        foreach (Combatant comb in battleList)
        {
            if(comb.hp>0) comb.attackEnemy();
            updateHealthBars();
            yield return new WaitForSeconds(1);
        }

        currentPartyIndex = 0;
        battleList.Clear();
        battleCo = StartCoroutine(battleProcess());
        yield break;
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

    public void updateHealthBars() {
        for (int i = 0; i < party.Length; i++)
        {
            partyHPSliders[i].value = (float)party[i].hp / (float)party[i].maxHp;
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].value = (float)enemies[i].hp / (float)enemies[i].maxHp;
        }

    }
    public void enableHealthBars()
    {
        partyHPBox.SetActive(true);
        enemyHPBox.SetActive(true);

        for(int i = 0; i < party.Length; i++)
        {
            partyHPSliders[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(true);
            attackTargetButtons[i].SetActive(true);
            rizzTargetButtons[i].SetActive(true);
        }
    }
    public void disableHealthBars()
    {
        partyHPBox.SetActive(false);
        enemyHPBox.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            partyHPSliders[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 4; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(false);
            attackTargetButtons[i].SetActive(false);
            rizzTargetButtons[i].SetActive(false);
        }
    }
    public void openAttackPanel()
    {
        attackTargetPanel.SetActive(true);
    }
    public void closeAttackPanel()
    {
        attackTargetPanel.SetActive(false);
    }

    public void openRizzPanel()
    {
        rizzTargetPanel.SetActive(true);
    }
    public void closeRizzPanel()
    {
        rizzTargetPanel.SetActive(false);
    }
    public void setMoveList(int partyIndex)
    {
        for (int i = 0; i < party[partyIndex].attackList.Count; i++)
        {
            attackMoveButtons[i].setMoves(this, partyIndex, i, false);
        }
        for (int i = 0; i < party[partyIndex].rizzAttackList.Count; i++)
        {
            rizzMoveButtons[i].setMoves(this, partyIndex, i, true);
        }
    }
    public void clearMoveList()
    {
        foreach(MoveButton button in attackMoveButtons)
        {
            button.attack = null;
            button.label.text = "";
        }
        foreach (MoveButton button in rizzMoveButtons)
        {
            button.attack = null;
            button.label.text = "";
        }
    }
}