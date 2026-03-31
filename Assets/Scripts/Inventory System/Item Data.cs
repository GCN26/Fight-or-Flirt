using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory System/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject model;
    public enum itemType
    {
        none,
        armor,
        weapon,
        heal,
        item
    }
    public itemType myType;
    [TextArea]
    public string description;
    public int attack, defense, speed, charisma;
    //Optional move index for weapons
    public int moveIndex;
    //For use with non-equip items
    public string objName;
    public string scriptName;
    public string funcName;
}
