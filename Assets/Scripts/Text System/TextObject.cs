using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

[Serializable]
public class TextObject
{
    public int id;
    public string dialogue;
    public string speaker_name;
    public int next_id;

    //choices
    public List<FuncEvent> functions;
    public List<TransformEvent> transforms;

    public void setVars()
    {
        var prefix = DialogueArrayManager.objArr.data[id];

        dialogue = prefix.dialogue;
        speaker_name = prefix.speaker_name;
        next_id = prefix.next_id;

        int loopF = 0;
        int loopT = 0;
        foreach(var fn in DialogueArrayManager.objArr.data[id].functions)
        {
            functions.Add(fn.setVars(id, loopF));
            loopF++;
        }
        foreach(var tr in DialogueArrayManager.objArr.data[id].transforms)
        {
            transforms.Add(tr.setVars(id, loopT));
            loopT++;
        }
    }
}

[Serializable]
public class TextObjectArr
{
    public TextObject[] data;
}

public static class DialogueArrayManager
{
    static public TextObjectArr objArr;
}

[Serializable]
public class FuncEvent
{
    public string obj_name;
    public string script_name;
    public string func_name;
    public int[] args;

    public FuncEvent setVars(int id, int index)
    {
        var prefix = DialogueArrayManager.objArr.data[id].functions[index];

        obj_name = prefix.obj_name;
        script_name = prefix.script_name;
        func_name = prefix.func_name;
        //ONLY ACCEPTS INTS AT THE MOMENT
        args = prefix.args;
        return this;
    }
    public void callFunc()
    {
        var funcObj = GameObject.Find(obj_name).GetComponent(script_name);
        var funcCompType = funcObj.GetType();
        var funcMethod = funcCompType.GetMethod(func_name);
        var argsA = new object[args.Length];
        for(int i = 0; i < argsA.Length; i++)
        {
            argsA[i] = args[i];
        }

        funcMethod.Invoke(funcObj, argsA);
    }
}

[Serializable]
public class TransformEvent
{
    public string obj_name;
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

    public Vector3 move_vec;
    public float move_speed;
    public interpType move_interp_type;
    public moveType move_target_type;

    public Vector3 rotation_vec;
    public float rot_speed;
    public interpType rot_interp_type;
    public rotType rot_target_type;

    public GameObject obj;
    Vector3 originPos, targetPos;
    Vector3 originRot, targetRot;

    public bool reached;
    bool mReach, rReach;

    float timerMove;
    float timerRot;

    public TransformEvent setVars(int id, int index)
    {
        var prefix = DialogueArrayManager.objArr.data[id].transforms[index];

        obj_name = prefix.obj_name;
        move_vec = prefix.move_vec;
        move_speed = prefix.move_speed;
        move_interp_type = prefix.move_interp_type;
        move_target_type = prefix.move_target_type;
        rotation_vec = prefix.rotation_vec;
        rot_speed = prefix.rot_speed;
        rot_interp_type = prefix.rot_interp_type;
        rot_target_type = prefix.rot_target_type;

        obj = GameObject.Find(obj_name);
        originPos = obj.transform.position;
        originRot = obj.transform.eulerAngles;

        switch (move_target_type)
        {
            case moveType.addTo: targetPos = originPos + move_vec; break;
            case moveType.setTo: targetPos = move_vec; break;
        }
        switch (rot_target_type)
        {
            case rotType.addTo: targetRot = originRot + rotation_vec; break;
            case rotType.setTo: targetRot = rotation_vec; break;
            case rotType.lookAt:
                var dir = (rotation_vec - originPos).normalized;
                obj.transform.LookAt(rotation_vec);
                targetRot = obj.transform.eulerAngles;
                obj.transform.eulerAngles = originRot;
                break;
        }
        return this;
    }

    public void resetVars()
    {
        clearReaches();

        obj_name = "";
        move_vec = Vector3.zero;
        move_speed = 1;
        move_interp_type = interpType.snap;
        move_target_type = moveType.addTo;
        rotation_vec = Vector3.zero;
        rot_speed = 1;
        rot_interp_type = interpType.snap;
        rot_target_type = rotType.addTo;

        obj = null;
        originPos = Vector3.zero;
        originRot = Vector3.zero;
        targetPos = Vector3.zero;
        targetRot = Vector3.zero;
    }

    public void clearReaches()
    {
        reached = false;
        rReach = false;
        mReach = false;
    }

    //have this in update
    public void transformProcess()
    {
        if (!reached)
        {
            //Change to FixedDeltaTime if needed
            timerMove += Time.deltaTime*move_speed;
            timerRot += Time.deltaTime * rot_speed;

            if (move_interp_type == interpType.linear)
            {
                obj.transform.position = Vector3.Lerp(originPos, targetPos, timerMove);
            }
            else if (move_interp_type == interpType.smooth)
            {
                obj.transform.position = Vector3.Lerp(originPos, targetPos, Mathf.SmoothStep(0.0f, 1.0f, timerMove));
            }
            else if (move_interp_type == interpType.snap)
            {
                obj.transform.position = targetPos;
            }

            if(rot_interp_type == interpType.linear)
            {
                obj.transform.eulerAngles = Vector3.Lerp(originRot, targetRot, timerRot);
            }
            else if (rot_interp_type == interpType.smooth)
            {
                obj.transform.eulerAngles = Vector3.Lerp(originRot, targetRot, Mathf.SmoothStep(0.0f, 1.0f, timerRot));
            }
            else if(rot_interp_type == interpType.snap)
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
        if((rReach && mReach) || (rReach && move_vec == Vector3.zero) || (mReach && rotation_vec == Vector3.zero))
        {
            reached = true;
        }
    }

    public class choiceEvent
    {
        public List<string> labels;
        public List<int> ids;


    }
}