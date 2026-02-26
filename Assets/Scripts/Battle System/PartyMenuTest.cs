using UnityEngine;

public class PartyMenuTest : MonoBehaviour
{
    public GameObject parentPanel;
    public PartyListObj[] childArray;
    public BattleManager battleManager;

    public void updateOrder()
    {
        for (int i = 0; i < childArray.Length; i++)
        {
            childArray[i].transform.SetSiblingIndex(i);
        }
    }
}
