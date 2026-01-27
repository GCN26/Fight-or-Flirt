using UnityEngine;

public class test : MonoBehaviour
{
    public string foo = "bar";

    public void hi(int i, int j)
    {
        Debug.Log(i+ j);
    }
    public void hi2()
    {
        Debug.Log(foo);
    }
}
