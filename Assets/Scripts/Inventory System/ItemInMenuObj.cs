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
    public TextMeshProUGUI buttonLabel;
    public Sprite iconNormal, iconSelect, iconClicked;

    public Color normalColor, highlightColor;

    public int index;

    private void OnEnable()
    {
        if(myType == itemType.item)
        {
            equipButton.gameObject.SetActive(true);
            buttonLabel.text = "Use";
        }
    }

    public void hoverItem()
    {
        label.color = highlightColor;
        iconImgObj.sprite = iconSelect;
    }
    public void unhoverItem()
    {
        label.color = normalColor;
        iconImgObj.sprite = iconNormal;
    }
}
