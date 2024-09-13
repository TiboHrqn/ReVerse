using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TInstanciator  
{

    public static GameObject Instantiate(GameObject gameObject)
    {
        GameObject go = Object.Instantiate(gameObject);
        RecSetup.SetupRecordersDuringGame(go);
        return go;
    }

    public static GameObject Instantiate(GameObject gameObject, Transform parent)
    {
        GameObject go = Object.Instantiate(gameObject, parent);
        RecSetup.SetupRecordersDuringGame(go);
        return go;
    }

    public static GameObject Instantiate(GameObject gameObject, Transform parent, bool instantiateInWorldSpace)
    {
        GameObject go = Object.Instantiate(gameObject, parent, instantiateInWorldSpace);
        RecSetup.SetupRecordersDuringGame(go);
        return go;
    }

    public static GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        GameObject go = Object.Instantiate(gameObject, position, rotation);
        RecSetup.SetupRecordersDuringGame(go);
        return go;
    }

    public static GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = Object.Instantiate(gameObject, position, rotation, parent);
        RecSetup.SetupRecordersDuringGame(go);
        return go;
    }



    
}
