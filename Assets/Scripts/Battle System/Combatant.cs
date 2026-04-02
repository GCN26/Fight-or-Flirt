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
    public int experience = 0;

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

    public bool isBoss;
    public bool isProtect;
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
        mandi,
        slimon,
        dot
    }
    public bossTypeChar characterType;

    public Combatant(string charName, int hp, int infat, int atk, int def, int speed, int charis, int level, int atkIndex0 = -1, int atkIndex1 = -1, int atkIndex2 = -1, int atkIndex3 = -1, int rizzIndex0 = -1, int rizzIndex1 = -1, int rizzIndex2 = -1, int rizzIndex3 = -1, int spriteIndex = 0, int flirtTypeA = 0, bool isBoss = false)
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

        this.isBoss = isBoss;

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

        isProtect = false;
        if(movePower == -1)
        {
            //Protect Move
            isProtect = true;
        }
        if (selectedAttack.barkListIndexes != -1)
        {
            Debug.Log(Attacks.barkListList[selectedAttack.barkListIndexes][UnityEngine.Random.Range(0, Attacks.barkListList[selectedAttack.barkListIndexes].Length-1)]);
        }

        float crit = 1;
        int random = UnityEngine.Random.Range(0, 16);
        if (random == 0) crit = 1.75f;
        int damage = (int)((movePower * attack) * crit / (target.defense));
        damage = (int)((float)damage * ((float)infatuation / (float)maxInfatuation));
        if (currentStatus == status.Burned) damage = (int)((float)damage * .75f);

        if (!target.isProtect)
        {
            if (isProtect) damage = -1;
            else
            {
                target.hp -= damage;
                Debug.Log(charName + " hits " + target.charName + " for " + damage.ToString() + " with " + selectedAttack.name);
                object[] objArr = new object[2];
                objArr[0] = target;
                objArr[1] = target.partyIndex;
                if (selectedAttack.secondaryEffect != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect).Invoke(selectedAttack, objArr);
                if (selectedAttack.secondaryEffect2 != "" && party) selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect2).Invoke(selectedAttack, objArr);
            }
        }
        else
        {
            damage = -2;
        }

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
        if (selectedAttack.secondaryEffect2 != "" && party) selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect2).Invoke(selectedAttack, objArr);
        string response = "They seem flattered.";
        if (matchType) response = "They seem really flustered!";
        return response;
    }
    public void equipStatChange()
    {
        if (party)
        {
            attack = baseAttack + weapon.itemType.attack;
            defense = baseDefense + armor.itemType.defense;
            speed = baseSpeed + weapon.itemType.speed + armor.itemType.speed;
            charisma = baseCharisma + weapon.itemType.charisma + armor.itemType.charisma;
        }
    }

    public void getLevel()
    {
        if (experience < 50) level = 1;
        else if (experience >= 50 && experience < 150) level = 2;
        else if (experience >= 150 && experience < 350) level = 3;
        else if (experience >= 350 && experience < 700) level = 4;
        else if(experience >= 700) level = 5;
    }
}

public static class Attacks
{
    public static Attack[] attackList =
    {
        new Attack("Slash","Using a weapon, the user slashes at the enemy.",10,0, barkListIndexes: 0), //0
        new Attack("Burning Cleave","Using magic, the user enhances their physical slash with fire.",20,0,"SecondEffectTest", barkListIndexes: 2), //1
        new Attack("Expert Stance","",12,0), //2
        new Attack("Cast","",10,0,barkListIndexes:9), //3
        new Attack("Fireball","",15,0,"SecondEffectTest",barkListIndexes:11), //4
        new Attack("Arcane Art","",35,0), //5
        new Attack("Smack","",5,0, barkListIndexes: 3), //6
        new Attack("Electric Lyre","",25,0), //7
        new Attack("Chord Strike","",10,0, barkListIndexes: 5), //8
        new Attack("Stab","",5,0,barkListIndexes:6), //9
        new Attack("Phantom Thief","",10,0,barkListIndexes:8), //10
        new Attack("Fleetfoot","",0,0), //11
        new Attack("Rock Slide","",15,0), //12
        new Attack("Earthquake","",25,0), //13
        new Attack("Sedimentary Slam","",25,0), //14
        new Attack("Spin Attack","",15,0), //15
        new Attack("Bludgeon","",10,0), //16
        new Attack("Slash","enemy variant of slash", 10,0), //17
        new Attack("Shield Up","",-1,0,barkListIndexes: 1), //18
        new Attack("Distract","",-1,0,barkListIndexes: 4), //19
        new Attack("Evade","",-1,0,barkListIndexes: 7), //20
        new Attack("Protection","",-1,0,barkListIndexes: 10), //21
    };
    public static Attack[] rizzList =
    {
        new Attack("Smooch","The user gives the enemy a kiss.",15,0,flirtType:1),
        new Attack("Talk Logically", "The user talks with thoughts to back up their words.",10,0,flirtType:3),
        new Attack("Speak from the Heart", "The user talks with emotions to back up their words.",10,0,flirtType:1),
        new Attack("Text Test","",10,0,"callTextFlirt",flirtType:1)
    };
    public static string[] warriorBarks0 =
    {
        //Warrior - Slash
        "Hah!",
        "By my sword!",
        "I've got this!",
        "Hyah!",
    };
    public static string[] warriorBarks1 =
    {
        //Warrior - Shield Up
        "I gotta be careful.",
        "Still standing.",
        "Time to defend.",
        "I can take it.",
    };
    public static string[] warriorBarks2 =
    {
        //Warrior - Burning Cleave
        "Burn!",
        "Hyah!",
        "Ignite!",
        "I won't back down!"
    };
    public static string[] bardBarks0 =
    {
        //Bard - Smack
        "Haha!",
        "Let's do this!",
        "Take this!",
        "My turn."
    };
    public static string[] bardBarks1 =
    {
        //Bard - Distract
        "Look over there!!",
        "Listen to this!",
        "You can't hit me!",
        "Don't get distracted!"
    };
    public static string[] bardBarks2 =
    {
        //Bard - Chord Strike
        "Too close?",
        "How's this sound?",
        "Listen closely...",
        "Whoops!"
    };
    public static string[] rogueBarks0 =
    {
        //Rogue - Stab
        "Hmph.",
        "Too late.",
        "Nowhere to run.",
        "Target acquired."
    };
    public static string[] rogueBarks1 =
    {
        //Rogue - Evade
        "Too slow.",
        "Watch out.",
        "Can you keep up?",
        "Try to catch me."
    };
    public static string[] rogueBarks2 =
    {
        //Rogue - Phantom Thief
        "I'll take this.",
        "Can't catch me?",
        "Don't look away.",
        "My turn."
    };
    public static string[] mageBarks0 =
    {
        //Mage - Cast
        "Oh?",
        "You better run.",
        "Got you!",
        "Too easy."
    };
    public static string[] mageBarks1 =
    {
        //Mage - Protection
        "Won't be that easy!",
        "Come at me!",
        "You're getting boring.",
        "Don't get cocky."
    };
    public static string[] mageBarks2 =
    {
        //Mage - Fireball
        "FIRE!",
        "Can't handle the heat?",
        "Too hot?",
        "Incinerate!",
        "Burn! Hahah! BURN!"
    };
    public static string[][] barkListList =
    {
        warriorBarks0,
        warriorBarks1,
        warriorBarks2,
        bardBarks0,
        bardBarks1,
        bardBarks2,
        rogueBarks0,
        rogueBarks1,
        rogueBarks2,
        mageBarks0,
        mageBarks1,
        mageBarks2
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

    public int barkListIndexes = -1;

    public Attack(string name, string desc, int power, int type, string secondaryEffect = "", string secondaryEffect2 = "", int flirtType = 0, int barkListIndexes = -1)
    {
        this.name = name;
        this.desc = desc;
        this.power = power;
        this.type = (AttackType)type;
        this.secondaryEffect = secondaryEffect;
        this.secondaryEffect2 = secondaryEffect2;

        this.barkListIndexes = barkListIndexes;
    }
    //Add additional effects as a switch statement

    public void SecondEffectTest(Combatant target,int index)
    {
        Debug.Log("Test Secondary Effect");
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        Debug.Log(index);
        battleMan.setEnemyStatus(target, index);
    }

    public void bossRockyAttackText(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.attackType = true;
        battleMan.holdForText = true;
    }
    public void bossRockyFlirtText(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.attackType = false;
        battleMan.holdForText = true;
    }
}

public static class enemyList
{
    public static Combatant[] enemyTable =
    {
        new Combatant("Rock Golem 1", 75, 100, 1, 2, 2, 2, 1, 17, spriteIndex: 13,flirtTypeA:1),
        new Combatant("Rock Golem 2", 75, 100, 1, 1, 2, 2, 1, 17, spriteIndex: 13,flirtTypeA:1),
        new Combatant("Rocky", 75, 100, 7, 2, 2, 2, 1, 17, spriteIndex: 4,flirtTypeA:1,isBoss: true),
        new Combatant("QR", 75, 100, 1, 1, 2, 2, 1, 17, spriteIndex: 6,flirtTypeA:1),
        new Combatant("Big Slime", 75, 100, 2, 2, 2, 2, 1, 17, spriteIndex: 11,flirtTypeA:1),
        new Combatant("Slime", 75, 100, 1, 1, 1, 1, 1, 17, spriteIndex: 12,flirtTypeA:1),
    };
    public static Combatant[] bossRecruitedTable =
    {
        new Combatant("Rocky", 100, 1, 5,10,3,3,1, spriteIndex:4,atkIndex0:0,atkIndex1:0,atkIndex2:0,atkIndex3:0,rizzIndex0:0,rizzIndex1:0,rizzIndex2:0,rizzIndex3:0),
    };
}

public static class encounterTables
{
    public static int[][] combatantIndexes = new int[][]
    {
        new int[] { 0, 1},
        new int[] {2},
        new int[] {4,5,5}
    };
    public static string[] battleStartMessages = new string[] {
        "A pair of rock golems block your path!",
        "Rocky blocks your path!",
        "A group of slimes blocks your path!"
    };
}