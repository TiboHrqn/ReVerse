using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class RecSetup 
{
    private static List<Tuple<Type, Type, ConstructorInfo>> typeInfos = new List<Tuple<Type, Type, ConstructorInfo>>();

    public static void LoadConfig(RecSetupConfig config)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        List<Type> allTypes = new List<Type>();
        foreach (Assembly assembly in assemblies)
        {
            Type[] assemblyTypes = assembly.GetTypes();
            allTypes.AddRange(assemblyTypes);
        }
        IEnumerable<Type> compRecTypes = allTypes.Where(t => t.IsSubclassOf(typeof(ComponentRecorder)));
        IEnumerable<Type> compTypes = allTypes.Where(t => t.IsSubclassOf(typeof(Component)));
        foreach(TypeNamePair typeNamePair in config.TypeNamePair)
        {
            Type compRecType =  compRecTypes.FirstOrDefault(t => t.Name.Equals(typeNamePair.CompRecName, StringComparison.InvariantCultureIgnoreCase));
            if (compRecType != null)
            {
                Type compType = compTypes.FirstOrDefault(t => t.Name.Equals(typeNamePair.CompName, StringComparison.InvariantCultureIgnoreCase));
                if (compType != null)
                {
                    ConstructorInfo constructor = compRecType.GetConstructor(new[] { compType });
                    if (constructor != null)
                    {
                        typeInfos.Add(new Tuple<Type, Type, ConstructorInfo>(compRecType, compType, constructor));
                    }
                    else
                    {
                        Debug.LogError($"The class '{typeNamePair.CompRecName}' lack a constructor taking only a '{typeNamePair.CompName}' as an argument.");
                    } 
                }
                else
                {
                    Debug.LogError($"Component type '{typeNamePair.CompName}' not found.");
                }
            }
            else
            {
                Debug.LogError($"Component recorder type '{typeNamePair.CompRecName}' not found.");
            }
        } 
    }

    public static void SetupRecorders(GameObject go)  
    {
        Recorder.Instance.AddGORecorder(new GORecorder(go));
        foreach (var info in typeInfos)
        { 
            Type compRecType = info.Item1;
            Type compType = info.Item2;
            ConstructorInfo constructor = info.Item3;

            foreach (Component component in go.GetComponents(compType))
            {
                ComponentRecorder recorder = (ComponentRecorder)constructor.Invoke(new object[] { component });
        
                if (recorder is ActiveComponentRecorder activeRecorder)
                {
                    Recorder.Instance.AddActiveCompRecorder(go, activeRecorder);
                }
                else if (recorder is PausableComponentRecorder pausableRecorder)
                {
                    Recorder.Instance.AddPausableCompRecorder(go, pausableRecorder);
                }
                else if (recorder is ActivePausableComponentRecorder activePausableRecorder)
                {
                    Recorder.Instance.AddActivePausableCompRecorder(go, activePausableRecorder);
                }
                else 
                {
                    Recorder.Instance.AddBasicCompRecorder(go, recorder);
                }
            }
        }
    }

    public static void SetupRecordersDuringGame(GameObject go)
    {
        if(go.HasTag("dontRec"))
        {
            return;
        }

        GORecorder goRecorder = new GORecorder(go);
        Recorder.Instance.AddGORecorder(goRecorder);
        Recorder.Instance.FirstRecordGO(go, goRecorder);

        foreach (var info in typeInfos)
        {
            Type compRecType = info.Item1;
            Type compType = info.Item2;
            ConstructorInfo constructor = info.Item3;

            foreach (Component component in go.GetComponents(compType))
            {
                ComponentRecorder recorder = (ComponentRecorder)constructor.Invoke(new object[] { component });
            
                if (recorder is ActiveComponentRecorder activeRecorder)
                {
                    Recorder.Instance.AddActiveCompRecorder(go, activeRecorder);
                    Recorder.Instance.FirstRecordComp(go, activeRecorder);
                }
                else if (recorder is PausableComponentRecorder pausableRecorder)
                {
                    Recorder.Instance.AddPausableCompRecorder(go, pausableRecorder);
                    Recorder.Instance.FirstRecordComp(go, pausableRecorder);
                }
                else if (recorder is ActivePausableComponentRecorder activePausableRecorder)
                {
                    Recorder.Instance.AddActivePausableCompRecorder(go, activePausableRecorder);
                    Recorder.Instance.FirstRecordComp(go, activePausableRecorder);
                }
                else 
                {
                    Recorder.Instance.AddBasicCompRecorder(go, recorder);
                    Recorder.Instance.FirstRecordComp(go, recorder);
                }
            }
        }

    }

    public static void SetupAllGO()
    {
        GameObject[] allRootObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true)
        .Where(obj => obj.transform.parent == null)
        .ToArray();
        foreach(GameObject go in allRootObjects)
        {
            SetupGO(go);
        }
    }

    public static void SetupGO(GameObject go)
    {
        if(!go.HasTag("dontRecord"))
        {
            SetupRecorders(go);
        }
        if(!go.HasTag("dontRecordChildren"))
        {
            foreach (Transform child in go.transform)
            {
                SetupGO(child.gameObject);
            }
        }
    }

}
