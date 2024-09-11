using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class LightRecorder : ComponentRecorder
{
    public Light light { get; private set; }

    public Color color { get; private set; }
    public float intensity { get; private set; }

    public LightRecorder(Light light)
    {
        this.light = light;
        this.color = light.color;
        this.intensity = light.intensity;
    }


    public override bool IsStateDifferent()
    {
        if(color != light.color || intensity != light.intensity)
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
        color = light.color;
        intensity = light.intensity;
    }

    protected override void EvenComponentState()
    {
        light.color = color;
        light.intensity = intensity;
    }


    protected override ComponentState GetRecorderState()
    {
        return new LightState(color, intensity);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is LightState lightState)
        {
            color = lightState.color;
            intensity = lightState.intensity;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if(stateA is LightState lightStateA && stateB is LightState lightStateB)
        {
            Color newColor = Color.Lerp(lightStateA.color, lightStateB.color, p);
            float newIntensity = Mathf.Lerp(lightStateA.intensity, lightStateB.intensity, p);
            
            return new LightState(newColor, newIntensity);
        }
        else 
        {
            throw new Exception("Wrong component state type");
        }
    }

    
}

public class LightState : ComponentState
{
    public Color color { get; private set; }
    public float intensity { get; private set; }

    public LightState(Color color, float intensity)
    {
        this.color = color;
        this.intensity = intensity;
    }

    public override string GetTrace()
    {
        
        return "{}";
    }
}
