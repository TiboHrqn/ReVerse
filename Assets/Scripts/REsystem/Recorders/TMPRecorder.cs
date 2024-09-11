using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using TMPro;

public class TMPRecorder : ComponentRecorder
{
    public TMP_Text tmp { get; private set; }

    public string text { get; private set; }

    public TMPRecorder(TMP_Text tmp)
    {
        this.tmp = tmp;
    }


    public override bool IsStateDifferent()
    {
        if(text != tmp.text)
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
        text = tmp.text;
    }

    protected override void EvenComponentState()
    {
        tmp.text = text;
    }


    protected override ComponentState GetRecorderState()
    {
        return new TMPState(text);
    }

    protected override void SetRecorderState(ComponentState compState)
    {
        if (compState is TMPState tmpState)
        {
            text = tmpState.text;
        }
        else
        {
            throw new Exception("Wrong component state type");
        }
    }

    protected override ComponentState LerpCompState(ComponentState stateA, ComponentState stateB, float p)
    {
        if(stateA is TMPState tmpStateA && stateB is TMPState tmpStateB)
        {
            return tmpStateA;  // on pourrait imaginer renvoyer un lerp si les 2 states sont très semblables (dans le cas d'affichage caractère par caractère par exemple)
        }
        else 
        {
            throw new Exception("Wrong component state type");
        }
    }

    
}

public class TMPState : ComponentState
{
    public string text { get; private set; }

    public TMPState(string text)
    {
        this.text = text;
        
    }

    public override string GetTrace()
    {
        
        return "{}";
    }
}
