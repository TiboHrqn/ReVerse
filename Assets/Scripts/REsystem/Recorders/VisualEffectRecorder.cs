using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System;

public class VisualEffectRecorder : ComponentRecorder
{
    public VisualEffect vfx { get; private set; }

    public bool isPlaying { get; private set; }

    public List<string> list;

    public VisualEffectRecorder(VisualEffect vfx)
    {
        this.vfx = vfx;
        list = new List<string>();
        vfx.GetSpawnSystemNames(list);
        //Changer le 0 si plusieurs syst�me de spawn
        isPlaying = vfx.GetSpawnSystemInfo(list[0]).playing;
    }

    public override bool IsStateDifferent()
    {
        if (isPlaying != vfx.GetSpawnSystemInfo(list[0]).playing)
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
        isPlaying = vfx.GetSpawnSystemInfo(list[0]).playing;
    }

    protected override void EvenComponentState()
    {
        if(IsStateDifferent())
        {
            if (isPlaying)
            {
                vfx.Play();
            }
            else
            {
                vfx.Stop();
            }
        }

    
    }

    protected override ComponentState GetRecorderState()
    {
        //Debug.Log(isPlaying);
        return new VisualEffectState(isPlaying);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is VisualEffectState vfxState)
        {
            isPlaying = vfxState.isPlaying;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if (stateA is VisualEffectState trStateA && stateB is VisualEffectState trStateB)
        {
            return stateA;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }


}

public class VisualEffectState : ComponentState
{
    public bool isPlaying { get; private set; }

    public VisualEffectState(bool isPlaying)
    {
        this.isPlaying = isPlaying;
    }
}
