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
    //Charisma: Player only, attack for flirting
    //Perception: Enemy only, defense for flirting
    public string charName;
    public int hp, infatuation, attack, defense, speed, charisma, perception;
    public int baseAttack, baseDefense, baseSpeed, baseCharisma, basePerception;
    public int maxHp, maxInfatuation;

    public int relationshipPoints;

    public int movePower;

    public int level = 1;

    public int attackTargetIndex;
    public int attackListIndex;

    public Combatant target;
    public int[] attackListIndexes = {-1,-1,-1,-1 };
    public int[] rizzAttackListIndexes = { -1, -1, -1, -1 };
    public List<Attack> attackList = new();
    public List<Attack> rizzAttackList = new();

    public enum flirtType
    {
        none,
        Kind,
        Shy,
        Asshole,
        Flirty,
        Masochist
    }
    public flirtType type;

    public ItemInstance armor,weapon;

    public Attack selectedAttack;

    public int battleSpriteIndex;
    public Sprite battleSprite;

    public bool party;
    public int partyIndex = -1;
    public enum type_of_attack
    {
        fight,
        flirt,
        status
    }
    public type_of_attack attackType;
    public enum status
    {
        Healthy,
        Burned,
        Poisoned,
        Confused,
        Dead
    }
    public status currentStatus;
    public enum bossTypeChar
    {
        none,
        rocky,
        b2,
        b3,
        b4,
        b5
    }
    public bossTypeChar characterType;

    public Combatant(string charName, int hp, int infat, int atk, int def, int speed, int charis, int level, int atkIndex0 = -1, int atkIndex1 = -1, int atkIndex2 = -1, int atkIndex3 = -1, int rizzIndex0 = -1, int rizzIndex1 = -1, int rizzIndex2 = -1, int rizzIndex3 = -1, int spriteIndex = 0, int flirtTypeA = 0)
    {
        this.charName = charName;
        this.hp = hp;
        this.maxHp = hp;
        this.infatuation = infat;
        this.maxInfatuation = infat;
        this.attack = atk;
        this.defense = def;
        this.speed = speed;
        this.charisma = charis;
        this.perception = charis;
        this.battleSpriteIndex = spriteIndex;
        this.type = (flirtType)flirtTypeA;

        baseAttack = attack;
        baseDefense = defense;
        baseSpeed = speed;
        baseCharisma = charisma;
        basePerception = charisma;

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
        this.perception = combB.perception;
        this.charisma = combB.charisma;
        this.battleSpriteIndex = combB.battleSpriteIndex;

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
        int damage = (int)((movePower * attack * level) * crit / (target.defense * target.level));
        damage = (int)((float)damage * ((float)infatuation / (float)maxInfatuation));
        if (currentStatus == status.Burned) damage = (int)((float)damage * .75f);
        target.hp -= damage;
        Debug.Log(charName + " hits " + target.charName + " for " + damage.ToString() + " with " + selectedAttack.name);
        object[] objArr = new object[2];
        objArr[0] = target;
        objArr[1] = target.partyIndex;
        if(selectedAttack.secondaryEffect != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect).Invoke(selectedAttack, objArr);
        if (selectedAttack.secondaryEffect2 != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect2).Invoke(selectedAttack, objArr);
        return damage;
    }
    public string rizzEnemy()
    {
        selectedAttack = rizzAttackList[attackListIndex];

        movePower = selectedAttack.power;

        int bonus = 3;
        bool matchType = ((int)target.type == (int)selectedAttack.type);
        if (matchType) bonus = 8;
        int rizz = (int)((float)(movePower * charisma * bonus)/(float)target.perception);
        target.infatuation -= rizz;
        Debug.Log(charName + " hits on " + target.charName + " with " + selectedAttack.name);
        Debug.Log(rizz);
        Debug.Log(bonus);
        object[] objArr = new object[2];
        objArr[0] = target;
        objArr[1] = target.partyIndex;
        if (selectedAttack.secondaryEffect != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect).Invoke(selectedAttack, objArr);
        if (selectedAttack.secondaryEffect2 != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect2).Invoke(selectedAttack, objArr);
        string response = "They seem flattered.";
        if (matchType) response = "They seem really flustered!";
        return response;
    }
    public void equipStatChange()
    {
        if (armor != null && weapon != null)
        {
            attack = baseAttack + weapon.itemType.attack;
            defense = baseDefense + armor.itemType.defense;
            speed = baseSpeed + weapon.itemType.speed + armor.itemType.speed;
            charisma = baseCharisma + weapon.itemType.charisma + armor.itemType.charisma;
        }
    }
}

public static class Attacks
{
    public static Attack[] attackList =
    {
        new Attack("Slash","Using a weapon, the user slashes at the enemy.",15,0),
        new Attack("Fire Slash","Using magic, the user enhances their physical slash with fire.",20,0,"SecondEffectTest"),
        new Attack("Tackle","The user tackles the enemy with their whole body.",10,0),
        new Attack("Punch","The user throws their equipment aside and just throws hands.",5,0),
        new Attack("Test","hi",10,0),
        new Attack("Text Test","",10,0,"callTextTest")
    };
    public static Attack[] rizzList =
    {
        new Attack("Smooch","The user gives the enemy a kiss.",15,0,flirtType:1),
        new Attack("Text Test","",10,0,"callTextFlirt",flirtType:1)
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
    public enum FlirtType
    {
        none,
        Kind,
        Shy,
        Asshole,
        Flirty,
        Masochist
    }
    public string secondaryEffect;
    public string secondaryEffect2;// Used mainly for text purposes

    public Attack(string name, string desc, int power, int type, string secondaryEffect="", string secondaryEffect2 = "", int flirtType = 0)
    {
        this.name = name;
        this.desc = desc;
        this.power = power;
        this.type = (AttackType)type;
        this.secondaryEffect = secondaryEffect;
        this.secondaryEffect2 = secondaryEffect2;
    }
    //Add additional effects as a switch statement

    public void SecondEffectTest(Combatant target,int index)
    {
        Debug.Log("Test Secondary Effect");
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        Debug.Log(index);
        battleMan.setEnemyStatus(target, index);
    }
    public void callTextTest(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.attackType = true;
        battleMan.test = true;
        //battleMan.textMan.callText(8);
    }
    public void callTextFlirt(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.attackType = false;
        battleMan.test = true;
        //battleMan.textMan.callText(8);
    }
}

public static class enemyList
{
    public static Combatant[] enemyTable =
    {
        new Combatant("Rock Golem 1", 75, 100, 2, 3, 4, 4, 1, 0, spriteIndex: 4,flirtTypeA:1),
        new Combatant("Rock Golem 2", 75, 100, 2, 3, 4, 4, 1, 0, spriteIndex: 5,flirtTypeA:1),
        new Combatant("Rocky", 100, 100, 5, 5, 4, 4, 1, 0, spriteIndex: 4,flirtTypeA:1),
    };
}

public static class encounterTables
{
    public static int[][] combatantIndexes = new int[][]
    {
        new int[] { 0, 1},
        new int[] {2}
    };
}