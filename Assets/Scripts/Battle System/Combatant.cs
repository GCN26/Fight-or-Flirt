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
    public int baseAttack, baseDefense, baseSpeed, baseLooks, baseIntelligence, baseCharisma;
    public int maxHp, maxInfatuation;

    public int movePower;

    public int level = 1;

    public int attackTargetIndex;
    public int attackListIndex;

    public Combatant target;
    public int[] attackListIndexes = {-1,-1,-1,-1 };
    public int[] rizzAttackListIndexes = { -1, -1, -1, -1 };
    public List<Attack> attackList = new();
    public List<Attack> rizzAttackList = new();

    public armor armor;
    public weapon weapon;

    public Attack selectedAttack;
    public enum type_of_attack
    {
        fight,
        flirt,
        status
    }
    public type_of_attack attackType;

    public Combatant(string charName, int hp, int infat, int atk, int def, int speed, int looks, int intel, int charis, int level, int atkIndex0 = -1, int atkIndex1 = -1, int atkIndex2 = -1, int atkIndex3 = -1, int rizzIndex0 = -1, int rizzIndex1 = -1, int rizzIndex2 = -1, int rizzIndex3 = -1)
    {
        this.charName = charName;
        this.hp = hp;
        this.maxHp = hp;
        this.infatuation = infat;
        this.maxInfatuation = infat;
        this.attack = atk;
        this.defense = def;
        this.speed = speed;
        this.looks = looks;
        this.intelligence = intel;
        this.charisma = charis;

        baseAttack = attack;
        baseDefense = defense;
        baseSpeed = speed;
        baseLooks = looks;
        baseIntelligence = intelligence;
        baseCharisma = charisma;

        if(atkIndex0 != -1) attackListIndexes[0] = atkIndex0;
        if (atkIndex1 != -1) attackListIndexes[1] = atkIndex1;
        if (atkIndex2 != -1) attackListIndexes[2] = atkIndex2;
        if (atkIndex3 != -1) attackListIndexes[3] = atkIndex3;

        if (rizzIndex0 != -1) rizzAttackListIndexes[0] = rizzIndex0;
        if (rizzIndex1 != -1) rizzAttackListIndexes[1] = rizzIndex1;
        if (rizzIndex2 != -1) rizzAttackListIndexes[2] = rizzIndex2;
        if (rizzIndex3 != -1) rizzAttackListIndexes[3] = rizzIndex3;

        equipStatChange();
    }

    public Combatant(Combatant combB)
    {
        this.charName = combB.charName;
        this.hp = combB.hp;
        this.maxHp = combB.hp;
        this.infatuation = combB.infatuation;
        this.maxInfatuation = combB.infatuation;
        this.attack = combB.attack;
        this.defense = combB.defense;
        this.speed = combB.speed;
        this.looks = combB.looks;
        this.intelligence = combB.intelligence;
        this.charisma = combB.charisma;

        attackListIndexes[0] = combB.attackListIndexes[0];
        attackListIndexes[1] = combB.attackListIndexes[1];
        attackListIndexes[2] = combB.attackListIndexes[2];
        attackListIndexes[3] = combB.attackListIndexes[3];

        rizzAttackListIndexes[0] = combB.rizzAttackListIndexes[0];
        rizzAttackListIndexes[1] = combB.rizzAttackListIndexes[1];
        rizzAttackListIndexes[2] = combB.rizzAttackListIndexes[2];
        rizzAttackListIndexes[3] = combB.rizzAttackListIndexes[3];

        equipStatChange();
    }


    public enum status
    {
        Healthy,
        Burned,
        Confused,
        Dead
    }

    public void getAttacksInList()
    {
        for(int i = 0; i < attackListIndexes.Length; i++)
        {
            if (attackListIndexes[i] != -1) attackList.Add(Attacks.attackList[attackListIndexes[i]]);
        }
        for(int i = 0; i <  rizzAttackListIndexes.Length; i++)
        {
            if(rizzAttackListIndexes[i] != -1) rizzAttackList.Add(Attacks.rizzList[rizzAttackListIndexes[i]]);
        }
    }

    public int attackEnemy()
    {
        selectedAttack = attackList[attackListIndex];

        movePower = selectedAttack.power;

        float crit = 1;
        int random = UnityEngine.Random.Range(0, 16);
        if (random == 0) crit = 1.75f;
        int damage = (int)((movePower*attack*level)*crit / (target.defense*target.level));
        target.hp -= damage;
        Debug.Log(charName + " hits " + target.charName + " for " + damage.ToString() + " with " + selectedAttack.name);
        return damage;
    }
    public int rizzEnemy()
    {
        selectedAttack = rizzAttackList[attackListIndex];

        movePower = selectedAttack.power;

        int rizz = (int)((movePower * charisma) * (looks / target.intelligence));
        target.infatuation -= rizz;
        Debug.Log(charName + " hits on " + target.charName + " for " + rizz.ToString() + " with " + selectedAttack.name);
        return rizz;
    }
    public void equipStatChange()
    {
        attack = baseAttack + weapon.attack;
        defense = baseDefense + armor.defense;
        speed = baseSpeed + weapon.speed + armor.speed;
        looks = baseLooks + weapon.looks + armor.looks;
        intelligence = baseIntelligence + weapon.intelligence + armor.intelligence;
        charisma = baseCharisma + weapon.charisma + armor.charisma;
    }
}

public static class Attacks
{
    public static Attack[] attackList =
    {
        new Attack("Slash","Using a weapon, the user slashes at the enemy.",15,0),
        new Attack("Fire Slash","Using magic, the user enhances their physical slash with fire.",20,0),
        new Attack("Tackle","The user tackles the enemy with their whole body.",10,0),
        new Attack("Punch","The user throws their equipment aside and just throws hands.",5,0)
    };
    public static Attack[] rizzList =
    {
        new Attack("Slash","Using a weapon, the user slashes at the enemy.",15,0),
        new Attack("Fire Slash","Using magic, the user enhances their physical slash with fire.",20,0),
        new Attack("Tackle","The user tackles the enemy with their whole body.",10,0),
        new Attack("Punch","The user throws their equipment aside and just throws hands.",5,0)
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

public static class enemyList
{
    public static Combatant[] enemyTable =
    {
        new Combatant("Test Enemy 1", 100, 100, 4, 4, 4, 4, 4, 4, 1, 0, 1, 2, 3),
        new Combatant("Test Enemy 2", 100, 100, 4, 4, 4, 4, 4, 4, 1, 0, 1, 2, 3),
        new Combatant("Speed 1", 100, 200, 4, 4, 1, 4, 4, 4, 1, 0, 1, 2, 3),
        new Combatant("Speed 3", 100, 200, 4, 4, 3, 4, 4, 4, 1, 0, 1, 2, 3),
        new Combatant("Speed 100", 100, 200, 4, 4, 100, 4, 4, 4, 1, 0, 1, 2, 3),
    };
}