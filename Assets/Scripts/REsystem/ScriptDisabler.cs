using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptDisabler : MonoBehaviour
{
    // SINGLETON PART
    private static ScriptDisabler instance = null;
    public static ScriptDisabler Instance => instance;
    //

    //DISABLER PART
    [SerializeField] private List<string> scriptNamesToDisable;

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

    public void FindScriptsToDisable()
    {
        foreach (string scriptClassName in scriptNamesToDisable)
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

    public void EnableScripts() // très bancal, car même les scripts initialement désactivés sont ractivés. pour palier à ce problème, il faudrait faire des recorder pour ces scripts aussi
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
