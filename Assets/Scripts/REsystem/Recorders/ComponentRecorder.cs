using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public abstract class ComponentRecorder 
{
    public abstract bool IsStateDifferent();

    protected abstract void EvenRecorderState();

    protected abstract void EvenComponentState();
    
    protected abstract ComponentState GetRecorderState();

    protected abstract void SetRecorderState(ComponentState compState);

    protected abstract ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p);


    private static float GetLerpRatio(float t1, float t2, float time)
    {
        if(t2 - t1 != 0f)
        {
            return (time - t1) / (t2 - t1);
            
        }
        else 
        {
            return 0;
        }
        
    }

    public ComponentState GetComponentState()
    {
        EvenRecorderState();
        return GetRecorderState();
    }

    public void SetCompState(ComponentState compState)
    {
        SetRecorderState(compState);
        EvenComponentState();
    }

    public void SetCompStateWithLerp (ComponentState stateA, ComponentState stateB, float t1, float t2, float time)
    {
        float p = GetLerpRatio(t1, t2, time);
        SetRecorderState(LerpCompState(stateA, stateB, p));
        EvenComponentState();
    }

}

public abstract class ActiveComponentRecorder : ComponentRecorder
{
    public abstract void EnableActiveComponent();
    public abstract void DisableActiveComponent();
}

public abstract class PausableComponentRecorder : ComponentRecorder
{
    public abstract void PauseComponent();
}

public abstract class ActivePausableComponentRecorder : ComponentRecorder
{
    public abstract void EnableActiveComponent();
    public abstract void DisableActiveComponent();
    public abstract void PauseComponent();
}