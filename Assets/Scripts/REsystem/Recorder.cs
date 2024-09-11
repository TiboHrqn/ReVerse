using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder 
{
    // SINGLETON PART
    private static Recorder instance;
    public static Recorder Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = new Recorder();
            }

            return instance;
        }
    }
    //RECORDER PART
    private HashSet<GORecorder> goRecorderSet;
    public Dictionary<GameObject, HashSet<ComponentRecorder>> activeCompRecorderDict { get; private set; }
    public Dictionary<GameObject, HashSet<ComponentRecorder>> basicCompRecorderDict { get; private set; }
    public Dictionary<GameObject, HashSet<ComponentRecorder>> pausableCompRecorderDict { get; private set; }
    public Dictionary<GameObject, HashSet<ComponentRecorder>> activePausableCompRecorderDict { get; private set; }
    public int nbRecord { get; private set; }
    private bool firstRecordDone;
    //

    private Recorder()
    {
        goRecorderSet = new HashSet<GORecorder>();
        activeCompRecorderDict = new Dictionary<GameObject, HashSet<ComponentRecorder>>();
        basicCompRecorderDict = new Dictionary<GameObject, HashSet<ComponentRecorder>>();
        pausableCompRecorderDict = new Dictionary<GameObject,HashSet<ComponentRecorder>>();
        activePausableCompRecorderDict = new Dictionary<GameObject,HashSet<ComponentRecorder>>();
        nbRecord = 0;
        firstRecordDone = false;
    }

    public void AddGORecorder(GORecorder goRecorder)
    {
        goRecorderSet.Add(goRecorder);
    }

    public void AddActiveCompRecorder(GameObject go, ComponentRecorder compRecorder)
    {
        if (activeCompRecorderDict.ContainsKey(go))
        {
            activeCompRecorderDict[go].Add(compRecorder);
        }
        else
        {
            activeCompRecorderDict.Add(go, new HashSet<ComponentRecorder>{compRecorder});
        }
    }

    public void AddBasicCompRecorder(GameObject go, ComponentRecorder compRecorder)
    {
        if (basicCompRecorderDict.ContainsKey(go))
        {
            basicCompRecorderDict[go].Add(compRecorder);
        }
        else
        {
            basicCompRecorderDict.Add(go, new HashSet<ComponentRecorder>{compRecorder});
        }
    }

    public void AddPausableCompRecorder(GameObject go, ComponentRecorder compRecorder)
    {
        if (pausableCompRecorderDict.ContainsKey(go))
        {
            pausableCompRecorderDict[go].Add(compRecorder);
        }
        else
        {
            pausableCompRecorderDict.Add(go, new HashSet<ComponentRecorder>{compRecorder});
        }
    }

    public void AddActivePausableCompRecorder(GameObject go, ComponentRecorder compRecorder)
    {
        if (activePausableCompRecorderDict.ContainsKey(go))
        {
            activePausableCompRecorderDict[go].Add(compRecorder);
        }
        else
        {
            activePausableCompRecorderDict.Add(go, new HashSet<ComponentRecorder>{compRecorder});
        }
    }

    private void FirstRecord(float time)
    {
        Storer.Instance.AddTimeData( new TimeRecord(nbRecord, time) );
        foreach (GORecorder goRecorder in goRecorderSet)
        {
            GameObject go = goRecorder.gameObject;
            FirstRecordGO(go, goRecorder);
            Storer.Instance.AddGOToSet(go);
            if (activeCompRecorderDict.ContainsKey(go))
            {
                foreach (ComponentRecorder compRecorder in activeCompRecorderDict[go])
                {
                    FirstRecordComp(go, compRecorder); ///
                    Storer.Instance.AddCompRecorderToDict(go, compRecorder);
                }    
            }
            if (basicCompRecorderDict.ContainsKey(go))
            {
                foreach (ComponentRecorder compRecorder in basicCompRecorderDict[go])
                {
                    FirstRecordComp(go, compRecorder); ///
                    Storer.Instance.AddCompRecorderToDict(go, compRecorder);
                }
            }
            if (pausableCompRecorderDict.ContainsKey(go))
            {
                foreach (ComponentRecorder compRecorder in pausableCompRecorderDict[go])
                {
                    FirstRecordComp(go, compRecorder); ///
                    Storer.Instance.AddCompRecorderToDict(go, compRecorder);
                }
            }
            if (activePausableCompRecorderDict.ContainsKey(go))
            {
                foreach (ComponentRecorder compRecorder in activePausableCompRecorderDict[go])
                {
                    FirstRecordComp(go, compRecorder); ///
                    Storer.Instance.AddCompRecorderToDict(go, compRecorder);
                }
            }
        }
    }

    public void Record(float time) 
    {
        nbRecord++;
        if(firstRecordDone)
        {
            Storer.Instance.AddTimeData( new TimeRecord(nbRecord, time) );
            foreach (GORecorder goRecorder in goRecorderSet)
            {
                GameObject go = goRecorder.gameObject;
                RecordGO(go, goRecorder);
                if (go.activeSelf)
                {
                    if (activeCompRecorderDict.ContainsKey(go))
                    {
                        foreach (ComponentRecorder compRecorder in activeCompRecorderDict[go])
                        {
                            RecordComp(go, compRecorder);
                        }
                    }
                    if (basicCompRecorderDict.ContainsKey(go))
                    {
                        foreach (ComponentRecorder compRecorder in basicCompRecorderDict[go])
                        {
                            RecordComp(go, compRecorder);
                        }
                    }
                    if (pausableCompRecorderDict.ContainsKey(go))
                    {
                        foreach (ComponentRecorder compRecorder in pausableCompRecorderDict[go])
                        {
                            RecordComp(go, compRecorder);
                        }
                    }
                    if (activePausableCompRecorderDict.ContainsKey(go))
                    {
                        foreach (ComponentRecorder compRecorder in activePausableCompRecorderDict[go])
                        {
                            RecordComp(go, compRecorder);
                        }
                    }
                }
            }
        }
        else
        {
            FirstRecord(time);
            firstRecordDone = true;
        }
    }

    public void RecordGO(GameObject go, GORecorder goRecorder)
    {
        if (goRecorder.IsStateDifferent())
        {
            GORecord goRecord = new GORecord(nbRecord, goRecorder.GetGOState());
            Storer.Instance.AddGORecord(go, goRecorder, goRecord);
        }
    }

    public void FirstRecordGO(GameObject go, GORecorder goRecorder)
    {
        GORecord goRecord = new GORecord(nbRecord, goRecorder.GetGOState());
        Storer.Instance.AddGORecord(go, goRecorder, goRecord);
    }

    public void RecordComp(GameObject go, ComponentRecorder compRecorder)
    {
        if(compRecorder.IsStateDifferent())
        {
            ComponentRecord compRecord = new ComponentRecord(nbRecord, compRecorder.GetComponentState());
            Storer.Instance.AddCompRecord(go, compRecorder, compRecord);
        }
    }

    public void FirstRecordComp(GameObject go, ComponentRecorder compRecorder)
    {
        ComponentRecord compRecord = new ComponentRecord(nbRecord, compRecorder.GetComponentState());
        Storer.Instance.AddCompRecord(go, compRecorder, compRecord);
    }
}
