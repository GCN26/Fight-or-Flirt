using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    public int[] indexes;
    public BattleManager battleManager;
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("COllision");
        if (other.gameObject.GetComponent<CharacterMovementScript>() != null)
        {
            battleManager.enemyTableIndex = indexes[UnityEngine.Random.Range(0, indexes.Length)];
            battleManager.startBattle();
            Destroy(this.gameObject);
        }
    }
}
