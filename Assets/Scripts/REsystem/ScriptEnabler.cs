using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptEnabler : MonoBehaviour
{
    // SINGLETON PART
    private static ScriptEnabler instance = null;
    public static ScriptEnabler Instance => instance;
    //

    //DISABLER PART
    [SerializeField] private List<string> scriptNamesToEnable;

    private List<MonoBehaviour> scriptsList = new List<MonoBehaviour>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);   
    }

    public void FindScriptsToEnable()
    {
        foreach (string scriptClassName in scriptNamesToEnable)
        {

            Type scriptType = Type.GetType(scriptClassName);

            if (scriptType != null && typeof(MonoBehaviour).IsAssignableFrom(scriptType))
            {
                MonoBehaviour[] scripts = FindObjectsOfType(scriptType) as MonoBehaviour[];
                if (scripts != null)
                {
                    scriptsList.AddRange(scripts);
                }
            } 
        }
    }

    public void DisableScripts()
    {
        foreach (MonoBehaviour script in scriptsList)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    public void EnableScripts() 
    {
        foreach (MonoBehaviour script in scriptsList)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }

    
}
