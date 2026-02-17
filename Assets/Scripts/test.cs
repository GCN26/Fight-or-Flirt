using UnityEngine;

public class test : MonoBehaviour
{
    public int foo = 1;
    public int bar = 1;

    public void hi(int i, int j)
    {
        Debug.Log(i+ j);
    }
    public void hi2()
    {
        Debug.Log(foo);
    }
}
