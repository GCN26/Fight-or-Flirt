using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

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
        var funcObj = GameObject.Find(objectName).GetComponent(objectScriptName);
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
    public enum interpType
    {
        snap,
        linear,
        smooth
    }
    public enum moveType
    {
        addTo,
        setTo
    }
    public enum rotType
    {
        addTo,
        setTo,
        lookAt
    }

    public Vector3 moveVector;
    public float moveSpeed;
    public interpType moveInterp;
    public moveType moveTargetType;

    public Vector3 rotVector;
    public float rotSpeed;
    public interpType rotInterp;
    public rotType rotTargetType;

    public GameObject obj;
    Vector3 originPos, targetPos;
    Vector3 originRot, targetRot;

    public bool reached;
    bool mReach, rReach;

    float timerMove;
    float timerRot;

    public void getVars()
    {
        obj = GameObject.Find(objectName);
        originPos = obj.transform.position;
        originRot = obj.transform.eulerAngles;

        switch (moveTargetType)
        {
            case moveType.addTo: targetPos = originPos + moveVector; break;
            case moveType.setTo: targetPos = moveVector; break;
        }
        switch (rotTargetType)
        {
            case rotType.addTo: targetRot = originRot + rotVector; break;
            case rotType.setTo: targetRot = rotVector; break;
            case rotType.lookAt:
                var dir = (rotVector - originPos).normalized;
                obj.transform.LookAt(rotVector);
                targetRot = obj.transform.eulerAngles;
                obj.transform.eulerAngles = originRot;
                break;
        }
    }

    //have this in update
    public void transformProcess()
    {
        if (!reached)
        {
            //Change to FixedDeltaTime if needed
            timerMove += Time.deltaTime*moveSpeed;
            timerRot += Time.deltaTime * rotSpeed;

            if (moveInterp == interpType.linear)
            {
                obj.transform.position = Vector3.Lerp(originPos, targetPos, timerMove);
            }
            else if (moveInterp == interpType.smooth)
            {
                obj.transform.position = Vector3.Lerp(originPos, targetPos, Mathf.SmoothStep(0.0f, 1.0f, timerMove));
            }
            else if (moveInterp == interpType.snap)
            {
                obj.transform.position = targetPos;
            }

            if(rotInterp == interpType.linear)
            {
                obj.transform.eulerAngles = Vector3.Lerp(originRot, targetRot, timerRot);
            }
            else if (rotInterp == interpType.smooth)
            {
                obj.transform.eulerAngles = Vector3.Lerp(originRot, targetRot, Mathf.SmoothStep(0.0f, 1.0f, timerRot));
            }
            else if(rotInterp == interpType.snap)
            {
                obj.transform.eulerAngles = targetRot;
            }
        }
        if(timerMove >= 1)
        {
            mReach = true;
            timerMove= 1;
            obj.transform.position = targetPos;
        }
        if(timerRot >= 1)
        {
            rReach = true;
            timerRot = 1;
            obj.transform.eulerAngles = targetRot;
        }
        //Fix if needed
        if((rReach && mReach) || (rReach && moveVector == Vector3.zero) || (mReach && rotVector == Vector3.zero))
        {
            reached = true;
        }
    }
}