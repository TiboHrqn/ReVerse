using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GORecord //correspond a l'état d'un go a un moment t
{
    public int timeID { get; private set; }
    public bool isGOActive { get; private set; }

    public GORecord(int timeID, bool isGOActive)
    {
        this.timeID = timeID;
        this.isGOActive = isGOActive;
    }

}

public class GOInfos //correspond a l'état d'un go a tout moment enregistré (+ le recorder)
{
    public GORecorder goRecorder { get; private set; }     
    public List<GORecord> goRecords { get; private set; }

    public GOInfos(GORecorder goRecorder)
    {
        this.goRecorder = goRecorder;
        goRecords = new List<GORecord>();
    }

    public GOInfos(GORecorder goRecorder, GORecord goRecord)
    {
        this.goRecorder = goRecorder;
        goRecords = new List<GORecord>{ goRecord };
    }

    public GOInfos(GORecorder goRecorder, List<GORecord> goRecords)
    {
        this.goRecorder = goRecorder;
        this.goRecords = goRecords;
    }

    public void AddRecord(GORecord goRecord)
    {
        goRecords.Add(goRecord);
    }

    public bool GetLastStateBeforeTimeID(int timeID)
    {
        int left = 0;
        int right = goRecords.Count - 1;
        bool result = false;      //si timeID est avant le premier élément, cela signifie que l'objet n'était pas record a l'instant timeID. On renvoie donc false
        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (goRecords[mid].timeID <= timeID)
            {
                result = goRecords[mid].isGOActive;
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return result;

    }

}

public abstract class ComponentState //est héritée dans les scripts des différents ComponentRecorder, correspond a un état complet d'un component
{
    public virtual string GetTrace()
    {
        return "{\"type\": \"notImplemented\"}";
    }
}

public class ComponentRecord //correspond a l'état d'un component a un moment t
{
    public int timeID { get; private set; }
    public ComponentState compState { get; private set; }

    public ComponentRecord(int timeID, ComponentState compState)
    {
        this.timeID = timeID;
        this.compState = compState;
    }

    public string GetTrace()
    {
        return "{ \"timeID\":"+ timeID + ", \"infos\":" + compState.GetTrace() + "}";
    }
}

public class TimeRecord //donne l'heure a un temps t
{
    public int timeID { get; private set; }
    public float time { get; private set; }

    public TimeRecord(int timeID, float time)
    {
        this.timeID = timeID;
        this.time = time;
    }

    public string GetTrace()
    {
        var trace = new
        {
            ID = timeID,
            Time = time
        };
        return JsonConvert.SerializeObject(trace, Formatting.Indented);
    }
}









