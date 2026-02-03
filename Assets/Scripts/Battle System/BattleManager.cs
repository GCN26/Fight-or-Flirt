using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Combatant comb0, comb1;
    void Start()
    {
        comb0.attack = 10;
        comb0.defense = 5;
        comb0.speed = 5;
        comb0.hp = 20;
        comb0.looks = 8;
        comb0.intelligence = 10;
        comb0.charisma = 15;

        comb1.attack = 10;
        comb1.defense = 5;
        comb1.speed = 5;
        comb1.hp = 20;
        comb1.looks = 8;
        comb1.intelligence = 5;
        comb1.charisma = 1;
        comb1.infatuation = 20;


    }

    public void testFight()
    {
        comb0.attackEnemy(comb1);
    }
    public void testFlirt()
    {
        comb0.rizzEnemy(comb1);
    }
}
