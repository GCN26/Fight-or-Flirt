using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory System/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject model;
    [TextArea]
    public string description;
}
