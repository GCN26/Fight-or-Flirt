using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class TextObject
{
    public string dialogue;
    public string speakerName;

    public FuncEvent[] funcList;
    public TransformEvent[] transformList;

}

[Serializable]
public class FuncEvent
{
    public string objectName;
    public string objectScriptName;
    public string funcName;
    public int[] funcArgs;

    public void callFunc()
    {
        Debug.Log(objectName);
        Debug.Log(objectScriptName);
        var funcObj = GameObject.Find(objectName).GetComponent(objectScriptName);
        Debug.Log(funcObj.name);
        var funcCompType = funcObj.GetType();
        var funcMethod = funcCompType.GetMethod(funcName);
        var args = new object[funcArgs.Length];
        for(int i = 0; i < funcArgs.Length; i++)
        {
            args[i] = funcArgs[i];
        }

        funcMethod.Invoke(funcObj, args);
    }
}

[Serializable]
public class TransformEvent
{
    public string objectName;
    public Vector3 moveVector;
    public float moveSpeed;
    //interp type
    public Vector3 rotVector;
    public float rotSpeed;
    //interp type

}