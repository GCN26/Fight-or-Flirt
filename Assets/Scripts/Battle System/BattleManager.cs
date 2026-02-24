using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.GraphicsBuffer;

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
    public HPBars[] partyHPSliders;
    public HPBars[] enemyHPSliders;

    public GameObject attackTargetPanel;
    public GameObject rizzTargetPanel;
    //Add item/party target panel
    //Change type of array to buttons?
    public GameObject[] attackTargetButtons;
    public GameObject[] rizzTargetButtons;
    public TextMeshProUGUI[] attackTargetLabels, rizzTargetLabels;
    //get these into a class

    public GameObject attackMovePanel;
    public GameObject rizzMovePanel;

    public MoveButton[] attackMoveButtons;
    public MoveButton[] rizzMoveButtons;

    public TextEventManager textMan;
    void Start()
    {
        party[0].armor = itemTables.armorTable[2];
        //battleCo = StartCoroutine(battleProcess());
    }
    void Update()
    {
        textMan.charMove.battleAllowMove = !battleOpen;
        if (Input.GetKeyDown(KeyCode.F) && !battleOpen)
        {
            startBattle();
            party[0].hp = party[0].maxHp;
        }
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

            battleOpen = true;
            //Add way to customize encounters
            //Add encounter table for enemies
            foreach (Combatant partyMember in party)
            {
                partyMember.attackList.Clear();
                partyMember.rizzAttackList.Clear();
                partyMember.getAttacksInList();
                partyMember.party = true;
                partyMember.partyIndex = Array.IndexOf(party, partyMember);

                battleList.Add(partyMember);
            }
            foreach (Combatant enemy in enemies)
            {
                enemy.attackList.Clear();
                enemy.rizzAttackList.Clear();
                enemy.getAttacksInList();

                battleList.Add(enemy);
            }
            for(int i = 0; i < 4; i++)
            {
                attackTargetButtons[i].GetComponent<Button>().interactable = true;
                rizzTargetButtons[i].GetComponent<Button>().interactable = true;
            }

            battleList = battleList.OrderBy(x => x.speed).ToList();
            battleList.Reverse();

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
        disableHealthBars();
        enableHealthBars();
        updateHealthBars();

        int partyDeadInt = 0;
        foreach (Combatant comb in party)
        {
            if (comb.hp > 0)
            {

            }
            else
            {
                partyDeadInt++;
            }
        }
        if (party.Length == partyDeadInt)
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

        if (battleList[0].party == true)
        {
            if (battleList[0].hp > 0)
            {
                battleUI.SetActive(true);
                currentPartyIndex = battleList[0].partyIndex;
                pausedForInput = true;
                battleList[0].target = null;
                battleUI.SetActive(true);
                setMoveList(Array.IndexOf(party, battleList[0]));
                while (pausedForInput) yield return null;

                closeAttackTargetPanel();
                closeRizzTargetPanel();
                closeAttackMovePanel();
                closeRizzMovePanel();
                pausedForInput = true;
            }
        }
        else
        {
            if (battleList[0].hp > 0 && battleList[0].infatuation > 0)
            {
                battleList[0].target = party[checkSlots(UnityEngine.Random.Range(0, party.Length))];
            }
        }
        battleUI.SetActive(false);

        textMan.battleText = true;
        if (battleList[0].hp > 0 && battleList[0].infatuation > 0)
        {
            switch (battleList[0].attackType)
            {
                case Combatant.type_of_attack.fight:
                    int damage = battleList[0].attackEnemy();
                    textMan.battleTextString = battleList[0].charName + " hits " + battleList[0].target.charName + " for " + damage.ToString() + " with " + battleList[0].selectedAttack.name;
                    break;
                case Combatant.type_of_attack.flirt:
                    int rizz = battleList[0].rizzEnemy();
                    textMan.battleTextString = battleList[0].charName + " hits on " + battleList[0].target.charName + " for " + rizz.ToString() + " with " + battleList[0].selectedAttack.name;
                    break;
            }
        }
        else textMan.battleTextString = battleList[0].charName + " is unable to fight!";
        updateHealthBars();
        textMan.startBattleText();
        yield return new WaitForSeconds(.5f);
        while (!textMan.progressable) yield return null;
        textMan.progressable = false;
        yield return new WaitForSeconds(.5f);

        Combatant temp = battleList[0];
        battleList.RemoveAt(0);
        battleList.Add(temp);

        textMan.endBattleText();
        battleCo = StartCoroutine(battleProcess());
        yield break;
    }

    IEnumerator battleProcessOld()
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

                closeAttackTargetPanel();
                closeRizzTargetPanel();
                closeAttackMovePanel();
                closeRizzMovePanel();
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

        //Get Speed for turn order
        battleList = battleList.OrderBy(x => x.speed).ToList();
        battleList.Reverse();
        foreach (Combatant comb in battleList)
        {
            textMan.battleText = true;
            if (comb.hp > 0 && comb.infatuation > 0)
            {
                switch (comb.attackType)
                {
                    case Combatant.type_of_attack.fight:
                        int damage = comb.attackEnemy();
                        textMan.battleTextString = comb.charName + " hits " + comb.target.charName + " for " + damage.ToString() + " with " + comb.selectedAttack.name;
                        break;
                    case Combatant.type_of_attack.flirt:
                        int rizz = comb.rizzEnemy();
                        textMan.battleTextString = comb.charName + " hits on " + comb.target.charName + " for " + rizz.ToString() + " with " + comb.selectedAttack.name;
                        break;
                }
            }
            else textMan.battleTextString = comb.charName + " is unable to fight!";
            updateHealthBars();
            textMan.startBattleText();
            yield return new WaitForSeconds(.5f);
            while (!textMan.progressable) yield return null;
            textMan.progressable = false;
            yield return new WaitForSeconds(.5f);
        }

        textMan.endBattleText();
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
            partyHPSliders[i].hpSlider.value = (float)party[i].hp / (float)party[i].maxHp;
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].hpSlider.value = (float)enemies[i].hp / (float)enemies[i].maxHp;
            enemyHPSliders[i].infatSlider.value = (float)enemies[i].infatuation / (float)enemies[i].maxInfatuation;
            if (enemies[i].hp <= 0 || enemies[i].infatuation <= 0)
            {
                attackTargetButtons[i].GetComponent<Button>().interactable = false;
                rizzTargetButtons[i].GetComponent<Button>().interactable = false;
            }
        }

    }
    public void enableHealthBars()
    {
        partyHPBox.SetActive(true);
        enemyHPBox.SetActive(true);

        for(int i = 0; i < party.Length; i++)
        {
            partyHPSliders[i].gameObject.SetActive(true);
            for(int j = 0; j < party[i].attackList.Count; j++)
            {
                attackMoveButtons[j].button.gameObject.SetActive(true);
            }
            for (int j = 0; j < party[i].rizzAttackList.Count; j++)
            {
                rizzMoveButtons[j].button.gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(true);
            attackTargetButtons[i].SetActive(true);
            attackTargetLabels[i].text = enemies[i].charName;
            rizzTargetButtons[i].SetActive(true);
            rizzTargetLabels[i].text = enemies[i].charName;
        }
    }
    public void disableHealthBars()
    {
        partyHPBox.SetActive(false);
        enemyHPBox.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            partyHPSliders[i].gameObject.SetActive(false);
            for (int j = 0; j < 4; j++)
            {
                attackMoveButtons[j].button.gameObject.SetActive(false);
                rizzMoveButtons[j].button.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            enemyHPSliders[i].gameObject.SetActive(false);
            attackTargetButtons[i].SetActive(false);
            rizzTargetButtons[i].SetActive(false);
        }
    }
    public void openAttackMovePanel()
    {
        closeRizzTargetPanel();
        closeRizzMovePanel();
        attackMovePanel.SetActive(true);
    }
    public void closeAttackMovePanel()
    {
        attackMovePanel.SetActive(false);
    }
    public void openRizzMovePanel()
    {
        closeAttackTargetPanel();
        closeAttackMovePanel();
        rizzMovePanel.SetActive(true);
    }
    public void closeRizzMovePanel()
    {
        rizzMovePanel.SetActive(false);
    }

    public void openAttackTargetPanel()
    {
        closeRizzTargetPanel();
        attackTargetPanel.SetActive(true);
    }
    public void closeAttackTargetPanel()
    {
        attackTargetPanel.SetActive(false);
    }
    public void openRizzTargetPanel()
    {
        closeAttackTargetPanel();
        rizzTargetPanel.SetActive(true);
    }
    public void closeRizzTargetPanel()
    {
        rizzTargetPanel.SetActive(false);
    }
    public void moveSelectAttack(int index)
    {
        party[currentPartyIndex].attackListIndex = index;
        party[currentPartyIndex].attackType = Combatant.type_of_attack.fight;
        openAttackTargetPanel();
    }
    public void moveSelectRizz(int index)
    {
        party[currentPartyIndex].attackListIndex = index;
        party[currentPartyIndex].attackType = Combatant.type_of_attack.flirt;
        openRizzTargetPanel();
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