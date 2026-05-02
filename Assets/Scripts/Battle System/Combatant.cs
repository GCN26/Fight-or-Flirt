using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

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
        Rude,
        Cheerful,
        Serious,
        Shy,
        Flirty,
    }

    public flirtType type;
    public int typeInt;

    public ItemInstance armor,weapon;

    public Attack selectedAttack;

    public int battleSpriteIndex;
    public Sprite battleSprite;

    public bool party;
    public int partyIndex = -1;

    public bool isBoss;
    public bool isPlayer;
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
    public string specialString;

    public Combatant(string charName, int hp, int infat, int flirtTypeA, int atk, int def, int speed, int charis, int level, int atkIndex0 = -1, int atkIndex1 = -1, int atkIndex2 = -1, int atkIndex3 = -1, int rizzIndex0 = -1, int rizzIndex1 = -1, int rizzIndex2 = -1, int rizzIndex3 = -1, int spriteIndex = 0, bool isBoss = false)
    {
        this.charName = charName;
        this.hp = hp;
        maxHp = hp;
        infatuation = infat;
        maxInfatuation = infat;
        attack = atk;
        defense = def;
        this.speed = speed;
        charisma = charis;
        perception = charis;
        battleSpriteIndex = spriteIndex;
        typeInt = flirtTypeA;
        type = (flirtType)flirtTypeA;

        Debug.Log(type);
        Debug.Log(typeInt);

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
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        specialString = "";
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
        if (random == 0 && !battleMan.specialEventManager.willowFight) crit = 1.5f;
        int damage = (int)((movePower * attack) * crit / (target.defense))/2;
        damage = (int)((float)damage * ((float)infatuation / (float)maxInfatuation));
        if (movePower == 0) damage = 0;
        if (currentStatus == status.Burned) damage = (int)((float)damage * .75f);

        if (!party)
        {
            if (target.hp-damage<= 0 && !isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkKill(this) != -1)
            {
                battleMan.barkBubbleEnemy[partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkKill(this)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkKill(this)].Length)]);
            }
            else if (!isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkAttack(this) != -1)
            {
                battleMan.barkBubbleEnemy[partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkAttack(this)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkAttack(this)].Length)]);
            }
        }
        else
        {
            if (target.hp - damage <= 0 && !target.isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkDeath(target) != -1)
            {
                battleMan.barkBubbleEnemy[target.partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkDeath(target)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkDeath(target)].Length)]);
            }
            else if (!target.isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkHit(target) != -1)
            {
                battleMan.barkBubbleEnemy[target.partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkHit(target)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkHit(target)].Length)]);
            }
        }

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
                Debug.Log(selectedAttack.secondaryEffect2);
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
        specialString = "";
        selectedAttack = rizzAttackList[attackListIndex];

        movePower = selectedAttack.power;

        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        int bonus = 1;

        if (target.type != flirtType.none)
        {
            switch (target.type)
            {
                case flirtType.Rude:
                    //Add repsonse barks here
                    if (selectedAttack.fType == Attack.FlirtType.Flattery)
                    {
                        bonus = 2;
                    }
                    else if (selectedAttack.fType == Attack.FlirtType.Heartfelt)
                    {
                        bonus = 0;
                    }
                    else bonus = 1;
                    break;
                case flirtType.Cheerful:
                    if (selectedAttack.fType == Attack.FlirtType.Heartfelt)
                    {
                        bonus = 2;
                    }
                    else if (selectedAttack.fType == Attack.FlirtType.Logic)
                    {
                        bonus = 0;
                    }
                    else bonus = 1;
                    break;
                case flirtType.Serious:
                    if (selectedAttack.fType == Attack.FlirtType.Logic)
                    {
                        bonus = 2;
                    }
                    else if (selectedAttack.fType == Attack.FlirtType.Care)
                    {
                        bonus = 0;
                    }
                    else bonus = 1;
                    break;
                case flirtType.Shy:
                    if (selectedAttack.fType == Attack.FlirtType.Care)
                    {
                        bonus = 2;
                    }
                    else if (selectedAttack.fType == Attack.FlirtType.Flattery)
                    {
                        bonus = 0;
                    }
                    else bonus = 1;
                    break;
                case flirtType.Flirty:
                    if (selectedAttack.fType == Attack.FlirtType.Flattery) bonus = 2;
                    else if (selectedAttack.fType == Attack.FlirtType.Logic || selectedAttack.fType == Attack.FlirtType.Care) bonus = 0;
                    else bonus = 1;
                    break;
                default: break;
            }
        }

        Debug.Log(target.type);

        int rizz = (int)((float)(movePower * charisma * bonus*2)/(float)target.perception);
        if (movePower == 0) rizz = 0;

        target.infatuation -= rizz;
        Debug.Log(charName + " hits on " + target.charName + " with " + selectedAttack.name);
        Debug.Log(rizz);
        Debug.Log(bonus);
        object[] objArr = new object[2];
        objArr[0] = target;
        objArr[1] = target.partyIndex;
        Debug.Log(selectedAttack.secondaryEffect2);
        if (selectedAttack.secondaryEffect != "") selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect).Invoke(selectedAttack, objArr);
        if (selectedAttack.secondaryEffect2 != "" && party) selectedAttack.GetType().GetMethod(selectedAttack.secondaryEffect2).Invoke(selectedAttack, objArr);
        string response = "";

        if (bonus == 2)
        {
            response = "They seem really flustered!";
            battleMan.enemyFlirtReacts[target.partyIndex].ifGoodTrue = true;
            battleMan.enemyFlirtReacts[target.partyIndex].gameObject.SetActive(true);
            if (!target.isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkFlirtEffective(target) != -1) battleMan.barkBubbleEnemy[target.partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkFlirtEffective(target)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkFlirtEffective(target)].Length)]);

        }
        else if (bonus == 0)
        {
            response = "They did not appreciate that.";
            battleMan.enemyFlirtReacts[target.partyIndex].ifGoodTrue = false;
            battleMan.enemyFlirtReacts[target.partyIndex].gameObject.SetActive(true);
            if(!target.isBoss && !battleMan.specialEventManager.willowFight && !battleMan.specialEventManager.mrRatFight && getBarkFlirtResist(target) != -1) battleMan.barkBubbleEnemy[target.partyIndex].setStringAndAppearForABit(Attacks.barkListList[getBarkFlirtResist(target)][UnityEngine.Random.Range(0, Attacks.barkListList[getBarkFlirtResist(target)].Length)]);
        }
        
        if (rizz == 0)
        {
            response = specialString;
        }
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

    public int getBarkAttack(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 12;
            case flirtType.Cheerful: return 18;
            case flirtType.Serious: return 24;
            case flirtType.Shy: return 30;
            case flirtType.Flirty: return 36;
            default: return -1;
        }
    }
    public int getBarkHit(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 13;
            case flirtType.Cheerful: return 19;
            case flirtType.Serious: return 25;
            case flirtType.Shy: return 31;
            case flirtType.Flirty: return 37;
            default: return -1;
        }
    }
    public int getBarkFlirtEffective(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 14;
            case flirtType.Cheerful: return 20;
            case flirtType.Serious: return 26;
            case flirtType.Shy: return 32;
            case flirtType.Flirty: return 38;
            default: return -1;
        }
    }
    public int getBarkFlirtResist(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 15;
            case flirtType.Cheerful: return 21;
            case flirtType.Serious: return 27;
            case flirtType.Shy: return 33;
            case flirtType.Flirty: return 39;
            default: return -1;
        }
    }
    public int getBarkDeath(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 16;
            case flirtType.Cheerful: return 22;
            case flirtType.Serious: return 28;
            case flirtType.Shy: return 34;
            case flirtType.Flirty: return 40;
            default: return -1;
        }
    }
    public int getBarkKill(Combatant target)
    {
        switch (target.type)
        {
            case flirtType.none: return -1;
            case flirtType.Rude: return 17;
            case flirtType.Cheerful: return 23;
            case flirtType.Serious: return 29;
            case flirtType.Shy: return 35;
            case flirtType.Flirty: return 41;
            default: return -1;
        }
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
        new Attack("Smack","",12,0, barkListIndexes: 3), //6
        new Attack("Electric Lyre","",25,0), //7
        new Attack("Chord Strike","",10,0, barkListIndexes: 5), //8
        new Attack("Stab","",15,0,barkListIndexes:6), //9
        new Attack("Phantom Thief","",10,0,"stealMoney",barkListIndexes:8), //10
        new Attack("Fleetfoot","",0,0), //11
        new Attack("Rock Slide","",15,0), //12
        new Attack("Earthquake","",25,0), //13
        new Attack("Sedimentary Slam","",25,0), //14
        new Attack("Spin Attack","",12,0), //15
        new Attack("Bludgeon","",10,0), //16
        new Attack("Slash","enemy variant of slash", 25,0), //17 - Enemy Attack
        new Attack("Shield Up","",-1,0,barkListIndexes: 1), //18
        new Attack("Distract","",-1,0,barkListIndexes: 4), //19
        new Attack("Evade","",-1,0,barkListIndexes: 7), //20
        new Attack("Protection","",-1,0,barkListIndexes: 10), //21
        new Attack("Bite","willow attack", 0,0, "willowBite"), //22
        new Attack("Slash","Using a weapon, the user slashes at the enemy.",10,0,"willowAttacked"), //23 - Willow Fight Copy
        new Attack("Cast","",6,0,"willowAttacked"), //24 - Willow Fight Copy
        new Attack("Smack","",12,0,"willowAttacked"), //25 - Willow Fight Copy
        new Attack("Stab","",12,0,"willowAttacked"), //26 - Willow Fight Copy
        new Attack("Slam","",15,0,"rockyFirstAttack"), //27 - Rocky Fight Copy
        new Attack("Earthquake","",25,0,"rockyFirstAttack"), //28 - Rocky Fight Copy
        new Attack("rockySecondHalfAttack","",0,0), //29 - Rocky Fiht Move - Exclusive to Rocky so he doesn't attack
        new Attack("Flee","",0,0,"flee", barkListIndexes: 0), //30
        new Attack("Slam","", 30,0), //31 - Enemy Attack
        new Attack("Bone Club","", 35,0), //32 - Enemy Attack
        new Attack("an actual gun","", 40,0), //33 - Enemy Attack
        new Attack("Punch","",5,0), //34
    };
    public static Attack[] rizzList =
    {
        new Attack("Smooch","The user gives the enemy a kiss.",15,0,flirtType:Attack.FlirtType.Flattery), //0
        new Attack("Talk Logically", "The user talks with thoughts to back up their words.",10,0,flirtType:Attack.FlirtType.Logic), //1
        new Attack("Speak from the Heart", "The user talks with emotions to back up their words.",10,0,flirtType:Attack.FlirtType.Heartfelt), //2
        new Attack("Text Test","",10,0,"callTextFlirt",flirtType:Attack.FlirtType.Logic), //3
        new Attack("Talk","",0,0,"callMrRatText",flirtType:Attack.FlirtType.none), //4
        new Attack("Talk Logically", "Willow Fight Attack",0,0,"willowLogic",flirtType:Attack.FlirtType.Logic), //5
        new Attack("Speak from the Heart", "Willow Fight Attack",0,0,"willowHeart",flirtType:Attack.FlirtType.Heartfelt), //6
        new Attack("Hug", "The user shows they care with a hug.",10,0,flirtType:Attack.FlirtType.Care), //7
        new Attack("Talk","",0,0,"failRockyTalk",flirtType:Attack.FlirtType.none), //8 - Rocky Fight Copy - First Part of Battle
        new Attack("Apologize","",0,0,"succeedRockyTalk",flirtType:Attack.FlirtType.none), //9 - Rocky Fight Copy - Second Part of Battle
        
        new Attack("Smooch","",15,0,"rockyFirstFlirt",flirtType:Attack.FlirtType.Flattery), //10 - Rocky Fight Copy - Final Part
        new Attack("Speak from the Heart", "",10,0,"rockyFirstFlirt",flirtType:Attack.FlirtType.Heartfelt), //11 - Rocky Fight Copy - Final Part
        new Attack("Talk Logically", "",0,0,"rockyFirstFlirt",flirtType:Attack.FlirtType.Logic), //12 - Rocky Fight Copy - Final Part
        new Attack("Hug", "",10,0,"rockyFirstFlirt",flirtType:Attack.FlirtType.Care), //13 - Rocky Fight Copy - Final Part

        new Attack("Talk","",0,0,"failRockyTalk",flirtType:Attack.FlirtType.none), //14 - Rocky Fight Copy - Final Part (No Flirt)
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

    public static string[] rudeBarkAttack =
    {
        "You're too weak.",
        "You're asking for it!",
        "Too much for you to take?"
    };
    public static string[] rudeBarkHit =
    {
        "Don't get cocky.",
        "I barely felt that.",
        "Are you even trying?"
    };
    public static string[] rudeBarkFlirtEffective =
    {
        "I mean, you're not wrong...",
        "I guess, if you're saying that, you're not completely incompetent."
    };
    public static string[] rudeBarkFlirtResist =
    {
        "Does it look like I care?",
        "Come on, really?"
    };
    public static string[] rudeBarkDeath =
    {
        "Out of anyone to kill me, it really had to be you?",
        "You'll get what's coming to you..."
    };
    public static string[] rudeBarkKill =
    {
        "That's what you get for taking me on!",
        "Did you really think it would end differently?"
    };

    public static string[] cheerfulBarkAttack = {
        "Take this!",
        "Let me show you how it's done!",
        "I got you now!"
    };
    public static string[] cheerfulBarkHit = {
        "That won't stop me!",
        "Ouch!",
        "Don't stop giving it your all!"
    };
    public static string[] cheerfulBarkFlirtEffective = { "I could say the same!","You're fun, I like you!"};
    public static string[] cheerfulBarkFlirtResist = { "Why are you taking this so seriously?","Stop thinking so much."};
    public static string[] cheerfulBarkDeath = { "Is this really... the end?","At least it was fun while it lasted..."};
    public static string[] cheerfulBarkKill = { "Aww, dead already?","That was fun! I wish there were more humans around to kill..."};

    public static string[] seriousBarkAttack = { "I'll make this quick.","Let's not draw this out.","Die, human."};
    public static string[] seriousBarkHit = { "Ugh.","That was strong.","At least you're taking this seriously."};
    public static string[] seriousBarkFlirtEffective = { "I suppose that makes sense.","I guess I can hear you out."};
    public static string[] seriousBarkFlirtResist = { "I don't have time for this.","Are you looking down on me?"};
    public static string[] seriousBarkDeath = { "This is too soon...","I wasn't ready yet..."};
    public static string[] seriousBarkKill = { "At least that didn't take too long.","This outcome was inevitable."};

    public static string[] shyBarkAttack = { "I've got this.","Please don't scream.","I just want to get this over with."};
    public static string[] shyBarkHit = { "Ouch, ouch, ouch.","I can take this... yeah, I can,","I've gotta keep it together."};
    public static string[] shyBarkFlirtEffective = { "You... really care?","That means a lot to me."};
    public static string[] shyBarkFlirtResist = { "I know you don't mean that.","I don't wanna hear it..."};
    public static string[] shyBarkDeath = { "I don't... want to die...","I didn't stand a chance, did I?"};
    public static string[] shyBarkKill = { "Phew, that's finally over.","I don't want to deal with that again any time soon."};

    public static string[] flirtyBarkAttack = { "This is my power!","Don't take this personally~","Too hard?"};
    public static string[] flirtyBarkHit = { "Oh, I really felt that!","Nghhhh.","Come on, harder next time!"};
    public static string[] flirtyBarkFlirtEffective = { "Oh my, tell me more?","I could say the same about you~"};
    public static string[] flirtyBarkFlirtResist = { "Come on, that's so boring...","You need to loosen up."};
    public static string[] flirtyBarkDeath = { "At least it was by your hands~","This isn't how I wanted to go."};
    public static string[] flirtyBarkKill = { "You look good in death.","The stronger one always comes out on top~"};

    public static string[][] barkListList =
    {
        warriorBarks0, //0
        warriorBarks1, //1
        warriorBarks2, //2
        bardBarks0, //3
        bardBarks1, //4
        bardBarks2, //5
        rogueBarks0, //6
        rogueBarks1, //7
        rogueBarks2, //8
        mageBarks0, //9
        mageBarks1, //10
        mageBarks2, //11
        rudeBarkAttack, //12
        rudeBarkHit, //13
        rudeBarkFlirtEffective, //14
        rudeBarkFlirtResist, //15
        rudeBarkDeath, //16
        rudeBarkKill, //17
        cheerfulBarkAttack, //18
        cheerfulBarkHit, //19
        cheerfulBarkFlirtEffective, //20
        cheerfulBarkFlirtResist, //21
        cheerfulBarkDeath, //22
        cheerfulBarkKill, //23
        seriousBarkAttack, //24
        seriousBarkHit, //25
        seriousBarkFlirtEffective, //26
        seriousBarkFlirtResist, //27
        seriousBarkDeath, //28
        seriousBarkKill, //29
        shyBarkAttack, //30
        shyBarkHit, //31
        shyBarkFlirtEffective, //32
        shyBarkFlirtResist, //33
        shyBarkDeath, //34
        shyBarkKill, //35
        flirtyBarkAttack, //36
        flirtyBarkHit, //37
        flirtyBarkFlirtEffective, //38
        flirtyBarkFlirtResist, //39
        flirtyBarkDeath, //40
        flirtyBarkKill, //41
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
        Flattery,
        Heartfelt,
        Logic,
        Care
    }
    public FlirtType fType;
    public string secondaryEffect;
    public string secondaryEffect2;// Used mainly for text purposes

    public int barkListIndexes = -1;

    public Attack(string name, string desc, int power, int type, string secondaryEffect = "", string secondaryEffect2 = "", FlirtType flirtType = FlirtType.none, int barkListIndexes = -1)
    {
        this.name = name;
        this.desc = desc;
        this.power = power;
        this.type = (AttackType)type;
        this.fType = flirtType;
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

    public void callMrRatText(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.battleList[0].specialString = "a";
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
        battleMan.specialIndex = 86;
    }
    public void willowBite(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.battleList[0].specialString = "b";
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
        battleMan.specialIndex = 117;
        target.hp -= (target.maxHp / 2)+1;
    }
    public void willowAttacked(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        specMan.willowBattleState += 1;
        if (specMan.willowBattleState == 1 || (specMan.willowBattleState == 2 && specMan.firstWillowChoice != -1))
        {
            battleMan.specialIndex = 181;
            specMan.firstWillowChoice = -1;
        }
        else if (specMan.willowBattleState == 2)
        {
            specMan.afterBattleIndex = 188;
            battleMan.specialIndex = 187;
            specMan.secondWillowChoice = -1;
        }
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }
    public void willowLogic(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.battleList[0].specialString = "b";
        specMan.willowBattleState += 1;
        if (specMan.willowBattleState == 1)
        {
            battleMan.specialIndex = 159; //Logic
            specMan.firstWillowChoice = 1;
        }
        else if (specMan.willowBattleState == 2)
        {
            specMan.secondWillowChoice = 1;
            if (specMan.firstWillowChoice == specMan.secondWillowChoice)
            {
                battleMan.specialIndex = 171; // Logic -> Logic -> Death
            }
            else if(specMan.firstWillowChoice == 2) battleMan.specialIndex = 128; //Heart -> Logic -> Win
            else if (specMan.firstWillowChoice == -1) battleMan.specialIndex = 171;
        }
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }
    public void willowHeart(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.battleList[0].specialString = "b";
        specMan.willowBattleState += 1;
        if (specMan.willowBattleState == 1)
        {
            battleMan.specialIndex = 121; //Heart
            specMan.firstWillowChoice = 2;
        }
        else if (specMan.willowBattleState == 2)
        {
            specMan.secondWillowChoice = 2;
            if (specMan.firstWillowChoice == specMan.secondWillowChoice)
            {
                battleMan.specialIndex = 126; // Heart -> Heart -> Death
            }
            else if (specMan.firstWillowChoice == 1) battleMan.specialIndex = 165; //Logic -> Heart -> Win
            else if (specMan.firstWillowChoice == -1) battleMan.specialIndex = 126;
            //Add battle end condition
        }
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }

    public void rockyFirstAttack(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        if (!specMan.hasRockyAttackedYet)
        {
            specMan.hasRockyAttackedYet = true;
            battleMan.specialIndex = 215;
            battleMan.battleUI.SetActive(false);
            battleMan.holdForText = true;
        }
    }
    public void failRockyTalk(Combatant target, int index) {

            SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
            BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            battleMan.specialIndex = 223;
            battleMan.battleUI.SetActive(false);
            battleMan.holdForText = true;
        }
    public void succeedRockyTalk(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.specialIndex = 227;
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }
    public void rockyFirstFlirt(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        if (!specMan.flirtedWithRockyYet)
        {
            specMan.flirtedWithRockyYet = true;
            battleMan.specialIndex = 250;
            battleMan.battleUI.SetActive(false);
            battleMan.holdForText = true;
            specMan.afterBattleIndex = 252;
        }
    }
    public void rockyFightAfterCalm(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        if (!specMan.betrayAttack && specMan.rockyState == SpecialEventManager.rockyBattleState.postApology)
        {
            specMan.betrayAttack = true;
            battleMan.specialIndex = 271;
            battleMan.battleUI.SetActive(false);
            battleMan.holdForText = true;
            specMan.afterBattleIndex = 273;
        }
        else if(!specMan.betrayAttack && specMan.rockyState == SpecialEventManager.rockyBattleState.calmedDown)
        {
            specMan.betrayAttack = true;
            battleMan.specialIndex = 281;
            battleMan.battleUI.SetActive(false);
            battleMan.holdForText = true;
            specMan.afterBattleIndex = 273;
        }
    }
    public void finalRockyTalk(Combatant target, int index)
    {
        SpecialEventManager specMan = GameObject.Find("GameManager").GetComponent<SpecialEventManager>();
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.specialIndex = 276;
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }
    public void stealMoney(Combatant target, int index)
    {
        GameManager gameMan = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameMan.money += UnityEngine.Random.Range(5, 50);
    }
    public void flee(Combatant target, int index)
    {
        BattleManager battleMan = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleMan.specialIndex = 291;
        battleMan.battleUI.SetActive(false);
        battleMan.holdForText = true;
    }
}

public static class enemyList
{
    public static Combatant[] enemyTable =
    {
        new Combatant("Rock Golem 1", 45, 70,2, 1, 2, 2, 2, 1, 31, spriteIndex: 13),
        new Combatant("Rock Golem 2", 40, 70,4, 1, 1, 2, 2, 1, 31, spriteIndex: 13),
        new Combatant("Rock Golem", 75, 50,2, 4, 2, 2, 2, 1, 27,28, spriteIndex: 4,isBoss: true),
        new Combatant("QR", 75, 100,5, 1, 1, 2, 2, 1, 17, spriteIndex: 6),
        new Combatant("Big Slime", 50, 100,5, 2, 2, 2, 2, 1, 31, spriteIndex: 11),
        new Combatant("Slime", 35, 100,2, 1, 2, 1, 1, 1, 31, spriteIndex: 12),
        new Combatant("Mr. Rat", 60, 120,3, 2, 1, 1, 1, 1, 31, spriteIndex: 19),
        new Combatant("Skeleton", 20, 30,5, 2, 1, 1, 1, 1, 31, spriteIndex: 20),
        new Combatant("Swordeton", 21, 50,1, 3, 1, 1, 1, 1, 17, spriteIndex: 21),
        new Combatant("Skeleton", 20, 120,4, 1, 1, 1, 1, 1, 32, spriteIndex: 22),
        new Combatant("Spider", 45, 120,3, 1, 1, 1, 1, 1, 17, spriteIndex: 23),
        new Combatant("Ugly Mushroom", 75, 100,1, 3, 1, 3, 1, 1, 33, spriteIndex: 24),
        new Combatant("Big Slime", 50, 100,5, 2, 2, 2, 2, 1, 31, spriteIndex: 11),
        new Combatant("Skeleton", 50, 70,1, 2, 1, 1, 1, 1, 31, spriteIndex: 20),
        new Combatant("Willow", 30, 100, 2, 1, 1, 1, 1, 1, 22, spriteIndex: 26),
    };
    public static Combatant[] bossRecruitedTable =
    {
        new Combatant("Rocky", 100, 1,2, 5,10,3,3,1, spriteIndex:4,atkIndex0:0,atkIndex1:0,atkIndex2:0,atkIndex3:0,rizzIndex0:0,rizzIndex1:0,rizzIndex2:0,rizzIndex3:0),
    };
}

public static class encounterTables
{
    public static int[][] combatantIndexes = new int[][]
    {
        new int[] { 0, 1},
        new int[] {2},
        new int[] {4,5,5},
        new int[] {6 },
        new int[] {7,9,7 },
        new int[] {7,8,9 },
        new int[] {10, 10},
        new int[] {11},
        new int[] {12},
        new int[] {4},
        new int[] {4},
        new int[] {14},
    };
}