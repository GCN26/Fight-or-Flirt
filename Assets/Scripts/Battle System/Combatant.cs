using System;
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
    public int hp, infatuation, attack, defense, speed, looks, intelligence, charisma;
    int minAtk, minDef,minSpeed, minLooks, minInt, minRizz;

    public int level = 1;

    public void attackEnemy(Combatant enemy)
    {
        float crit = 1;
        int random = UnityEngine.Random.Range(0, 16);
        if (random == 0) crit = 1.75f;
        int damage = (int)((attack*level)*(intelligence/3)*crit / (enemy.defense*enemy.level));
        enemy.hp -= damage;
        Debug.Log(damage);
    }
    public void rizzEnemy(Combatant enemy)
    {
        int rizz = (int)(looks * (looks / enemy.intelligence));
        enemy.infatuation -= rizz;
        Debug.Log(rizz);
    }
}
