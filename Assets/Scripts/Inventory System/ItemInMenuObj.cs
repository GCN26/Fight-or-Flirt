using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInMenuObj : MonoBehaviour
{
    public string itemName;
    public Sprite icon;
    public Image iconImgObj;
    public TextMeshProUGUI label;
    public enum itemType
    {
        none,
        armor,
        weapon,
        heal,
        item
    }
    public itemType myType;
    public Button equipButton;
}
