using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GORecorder
{
    public GameObject gameObject { get; private set; }

    public bool isActive { get; private set; }
    

    public GORecorder(GameObject go)
    {
        gameObject = go;
        isActive = go.activeSelf;

    }

    public bool IsStateDifferent()
    {
        if(isActive != gameObject.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvenRecorderState()
    {
        isActive = gameObject.activeSelf;
    }

    private void EvenGOState()
    {
        gameObject.SetActive(isActive);
    }

    public void SetGOState(bool newState)
    {
        isActive = newState;
        EvenGOState();
    }

    public bool GetGOState()
    {
        EvenRecorderState();
        return gameObject.activeSelf;
    }

}
