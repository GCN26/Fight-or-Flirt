using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI label;
    public int index; //sets the value for selected attack index in the combatant object
    public Attack attack;
    bool isRizz;

    public void setMoves(BattleManager battleManager, int partyIndex, int buttonIndex, bool isRizz)
    {
        this.isRizz = isRizz;
        if (!isRizz)
        {
            attack = Attacks.attackList[battleManager.party[partyIndex].attackListIndexes[buttonIndex]];
        }
        else
        {
            attack = Attacks.rizzList[battleManager.party[partyIndex].rizzAttackListIndexes[buttonIndex]];
        }
        label.text = attack.name;
    }
}