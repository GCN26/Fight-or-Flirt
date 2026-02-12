using UnityEngine;
using UnityEngine.UIElements;

public class ItemDisplay : MonoBehaviour
{
    public int itemIndex;
    public Image image;

    public void UpdateItemDisplay(Sprite newSprite, int newIndex)
    {
        image.sprite = newSprite; 
        itemIndex = newIndex;
    }
}
