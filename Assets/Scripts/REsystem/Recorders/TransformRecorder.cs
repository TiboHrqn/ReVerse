using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class TransformRecorder : ComponentRecorder
{
    public Transform transform { get; private set; }

    public Vector3 localPosition { get; private set; }
    public Quaternion localRotation { get; private set; }
    public Vector3 localScale { get; private set; }
    public Transform parent { get; private set; }        //a voir si ca ne rend pas les données trop grosses de ne pas séparer le parent dans un autre recorder

    public TransformRecorder(Transform tr)
    {
        this.transform = tr;
        this.localPosition = tr.localPosition;
        this.localRotation = tr.localRotation;
        this.localScale = tr.localScale;
    }


    public override bool IsStateDifferent()
    {
        if(localPosition != transform.localPosition || localRotation != transform.localRotation || localScale != transform.localScale || parent != transform.parent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void EvenRecorderState()
    {
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        localScale = transform.localScale;
        parent = transform.parent;
    }

    protected override void EvenComponentState()
    {
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
        transform.localScale = localScale;
        transform.SetParent(parent, false);
    }


    protected override ComponentState GetRecorderState()
    {
        return new TransformState(localPosition, localRotation, localScale, parent);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is TransformState transformState)
        {
            localPosition = transformState.localPosition;
            localRotation = transformState.localRotation;
            localScale = transformState.localScale;
            parent = transformState.parent;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if(stateA is TransformState trStateA && stateB is TransformState trStateB)
        {
            Vector3 newPos = Vector3.Lerp(trStateA.localPosition, trStateB.localPosition, p);
            Quaternion newRot = Quaternion.Slerp(trStateA.localRotation, trStateB.localRotation, p);
            Vector3 newlocalScale = Vector3.Lerp(trStateA.localScale, trStateB.localScale, p);
            return new TransformState(newPos, newRot, newlocalScale, trStateA.parent);
        }
        else 
        {
            throw new Exception("Wrong component state type");
        }
    }

    
}

public class TransformState : ComponentState
{
    public Vector3 localPosition { get; private set; }
    public Quaternion localRotation { get; private set; }
    public Vector3 localScale { get; private set; }
    public Transform parent { get; private set; }

    public TransformState(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent)
    {
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
        this.parent = parent;
    }

    public override string GetTrace() //only get trace of position & rotation for the moment
    {
        
        return "{" + "\"Type\":\"Transform\",\"Position\":" + localPosition.ToString() +",\"Rotation\":" +localRotation.ToString("F2") + "}";
    }
}
