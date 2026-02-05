using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Combatant
{

    //HP: Damage til death
    //Infatuation: Enemy Only, HP but for flirting
    //Attack: Determines how much damage is done to the enemy through violence
    //Defense: Determines how much damage is resisted from attacks
    //Speed: Determines turn order
    //Looks: Player only, essentially flirting attack
    //Intelligence: essentially flirting defense
    //Charisma: Player only, modifier for infatuation
    public string charName;
    public int hp, infatuation, attack, defense, speed, looks, intelligence, charisma;
    int minAtk, minDef,minSpeed, minLooks, minInt, minRizz;

    public int movePower;

    public int level = 1;

    public int attackTargetIndex;
    public int attackListIndex;

    public Combatant target;
    public List<int> attackListIndexes;
    public List<Attack> attackList;

    Attack selectedAttack;

    public enum status
    {
        Healthy,
        Burned,
        Confused,
        Dead
    }

    public void getAttacksInList()
    {
        for(int i = 0; i < attackListIndexes.Count; i++)
        {
            attackList.Add(Attacks.attackList[attackListIndexes[i]]);
        }
    }

    public void attackEnemy()
    {
        selectedAttack = attackList[attackListIndex];

        movePower = selectedAttack.power;

        float crit = 1;
        int random = UnityEngine.Random.Range(0, 16);
        if (random == 0) crit = 1.75f;
        int damage = (int)((movePower*attack*level)*crit / (target.defense*target.level));
        target.hp -= damage;
        Debug.Log(charName + " hits " + target.charName + " for " + damage.ToString() + " with " + selectedAttack.name);
    }
    public void rizzEnemy()
    {
        selectedAttack = attackList[attackListIndex];

        movePower = selectedAttack.power;

        int rizz = (int)((movePower * charisma) * looks * (looks / target.intelligence));
        target.infatuation -= rizz;
        Debug.Log(rizz);
    }
}

public static class Attacks
{
    public static Attack[] attackList =
    {
        new Attack("Slash","Basic Sword Slash",15,0),
        new Attack("Fire Slash","Firey Sword Slash",115,0),
    };
}

[Serializable]
public class Attack
{
    public string name;
    public string desc;
    public int power;
    public enum AttackType
    {
        Fight,
        Flirt,
        Status
    }
    public AttackType type;
    //Bonus Effects

    public Attack(string name, string desc, int power, int type)
    {
        this.name = name;
        this.desc = desc;
        this.power = power;
        this.type = (AttackType)type;
    }
    //Add additional effects as a switch statement
}