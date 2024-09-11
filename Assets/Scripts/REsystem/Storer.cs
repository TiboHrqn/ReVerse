using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Storer
{
    // SINGLETON PART
    private static Storer instance;
    public static Storer Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = new Storer();
            }

            return instance;
        }
    }

    //STORER PART

    private HashSet<GameObject> trackedGOSet;
    private Dictionary<GameObject, HashSet<ComponentRecorder>> compRecorderDict;

    public float firstTime { get; private set; }
    public float lastTime { get; private set; }

    private RecordSegment firstSegment;
    private RecordSegment currentRecordSegment; 
    public List<RecordSegment> currentReviewSegments { get; private set; }

    private int prevRecSegIndex;   //n'est fiable qu'après avoir appelé IsTimeBetweenTwoSegments qui a renvoyé true

    private RecordSegment auxRecSeg;  //utilisé dans la fonction GetCurrentReplaySegment
    private float auxTime;            //utilisé dans la fonction GetCurrentReplaySegment

    public static Dictionary <int, RecordSegment> recSegDict { get; private set; }

    //

    private Storer()
    {
        recSegDict = new Dictionary<int, RecordSegment>();
        firstSegment = new RecordSegment();
        currentRecordSegment = firstSegment;
        currentReviewSegments = new List<RecordSegment>{firstSegment};
        trackedGOSet = new HashSet<GameObject>();
        compRecorderDict = new Dictionary<GameObject, HashSet<ComponentRecorder>>();
    }

    public void AddTimeData(TimeRecord timeRecord)
    {
        currentRecordSegment.AddTimeData(timeRecord);
    }

    public void AddGORecord(GameObject go, GORecorder goRecorder, GORecord goRecord)
    {
        currentRecordSegment.AddGORecord(go, goRecorder, goRecord);

    } 

    public void AddCompRecord(GameObject go, ComponentRecorder compRecorder, ComponentRecord compRecord)
    {
        currentRecordSegment.AddCompRecord(go, compRecorder, compRecord);
    }

    public void AddGOToSet(GameObject go)
    {
        trackedGOSet.Add(go);
    }

    public void AddCompRecorderToDict(GameObject go, ComponentRecorder compRecorder)
    {
        if (!compRecorderDict.ContainsKey(go))
        {
            compRecorderDict[go] = new HashSet<ComponentRecorder>();
        }
        compRecorderDict[go].Add(compRecorder);
    }



//-----------------------------------------------------------------------------------
//----------------------------------- REPLAY PART -----------------------------------
//-----------------------------------------------------------------------------------

    public void UpdateFirstLastTimeValue()
    {
        foreach (RecordSegment recordSegment in currentReviewSegments)
        {
            recordSegment.UpdateFirstLastTimeValue();
        }
        firstTime = currentReviewSegments[0].firstTime;
        lastTime = currentReviewSegments[currentReviewSegments.Count - 1].lastTime;
    }

    private RecordSegment GetCurrentReviewSegment(float time)
    {
        if (time == auxTime)
        {
            return auxRecSeg;
        }
        foreach (RecordSegment recSeg in currentReviewSegments)
        {
            if(recSeg.lastTime >=  time)
            {
                auxTime = time;
                auxRecSeg = recSeg;
                return recSeg;
            }
        }
        auxTime = time;
        auxRecSeg = currentReviewSegments[currentReviewSegments.Count - 1];
        return  auxRecSeg;
    }
    
    public void RestoreAllStates(float time)
    {
        RecordSegment currentRecSeg = GetCurrentReviewSegment(time);
        int prevTimeID = currentRecSeg.GetPrevTimeID(time);

        foreach (GameObject go in trackedGOSet)
        {
            if(currentRecSeg.goInfosDict.ContainsKey(go))
            {
                RestoreGOState(currentRecSeg.goInfosDict[go], prevTimeID);
            }
            if(currentRecSeg.compInfosDict.ContainsKey(go))
            {
                foreach (var item2 in currentRecSeg.compInfosDict[go])
                {
                    if (item2.Key is ActiveComponentRecorder activeCompRecorder)
                    {
                        activeCompRecorder.EnableActiveComponent();
                    }
                    RestoreCompState(currentRecSeg, item2.Key, item2.Value, prevTimeID, time);
                }
            }   
        }
    }

    public void RestoreUnactivesStates(float time)
    {
        RecordSegment currentRecSeg = GetCurrentReviewSegment(time);
        int prevTimeID = currentRecSeg.GetPrevTimeID(time);
        foreach (GameObject go in trackedGOSet)
        {
            if(currentRecSeg.goInfosDict.ContainsKey(go))
            {
                RestoreGOState(currentRecSeg.goInfosDict[go], prevTimeID);
            }
            if(currentRecSeg.compInfosDict.ContainsKey(go))
            {
                foreach (var item2 in currentRecSeg.compInfosDict[go])
                {
                    if(item2.Key is not ActiveComponentRecorder)
                    {
                        RestoreCompState(currentRecSeg, item2.Key, item2.Value, prevTimeID, time);
                    }
                }
            }   
        }
        
    }

    public void DisableActiveComponents()
    {
        foreach (var item in Recorder.Instance.activeCompRecorderDict)
        {
            if (item.Key.activeSelf)
            {
                foreach (ActiveComponentRecorder activeCompRec in item.Value)
                {
                    activeCompRec.DisableActiveComponent();
                }
                foreach(ActivePausableComponentRecorder activePausableCompRec in item.Value)
                {
                    activePausableCompRec.DisableActiveComponent();
                }
            }
            
        }
    }

    public void PausePausableComponents()
    {
        foreach (var item in Recorder.Instance.pausableCompRecorderDict)
        {
            if (item.Key.activeSelf)
            {
                foreach (PausableComponentRecorder pausableCompRec in item.Value)
                {
                    pausableCompRec.PauseComponent();
                }
                foreach(ActivePausableComponentRecorder activePausableCompRec in item.Value)
                {
                    activePausableCompRec.PauseComponent();
                }
            }  
        }
    }

    private void RestoreGOState(GOInfos goInfos, int prevTimeID)
    {
        goInfos.goRecorder.SetGOState(goInfos.GetLastStateBeforeTimeID(prevTimeID));
    }

    private void RestoreCompState(RecordSegment currentRepSeg, ComponentRecorder compRecorder, List<ComponentRecord> compRecords, int prevTimeID, float time )
    {
        Tuple<ComponentRecord, ComponentRecord> compRecTuple = GetCompRecordsAroundTimeID(compRecords, prevTimeID);
        if(compRecTuple.Item2.timeID - prevTimeID > 1)
        { 
            compRecorder.SetCompState(compRecTuple.Item1.compState);
        }
        else
        {
            float t1 = currentRepSeg.GetTimeFromID(compRecTuple.Item1.timeID);
            float t2 = currentRepSeg.GetTimeFromID(compRecTuple.Item2.timeID);
            compRecorder.SetCompStateWithLerp(compRecTuple.Item1.compState, compRecTuple.Item2.compState, t1, t2, time);
        }
    }

    private Tuple<ComponentRecord, ComponentRecord> GetCompRecordsAroundTimeID(List<ComponentRecord> compRecords, int timeID)
    {
        int left = 0;
        int right = compRecords.Count - 1;
        bool mostLeft = true;
        bool mostRight = true;
        ComponentRecord result = compRecords[0];      //if timeID is before the first element, we return the first element
        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (compRecords[mid].timeID <= timeID)
            {
                result = compRecords[mid];
                left = mid + 1;
                mostLeft = false;
            }
            else
            {
                right = mid - 1;
                mostRight = false;
            }
        }
        if(!mostLeft && !mostRight)
        {
            return Tuple.Create(result, compRecords[left]);
        }
        else
        {
            return Tuple.Create(result, result);
        }
    }

    public void Split(float time)
    {
        RecordSegment currentRecSeg =  GetCurrentReviewSegment(time);
        currentRecSeg.Split(time);
        currentRecordSegment = new RecordSegment(); 
        currentRecSeg.AddNextSegment(currentRecordSegment);
    }

    public void WriteStorerTrace()
    {
        //Use RecordSegment.WriteSegmentTrace()
    }

//-----------------------------------------------------------------------------------
//-------------------- METHODS TO MODIFY CURRENT RECORD SEGMENTS --------------------
//-----------------------------------------------------------------------------------

    public void RemoveLastSegmentInReview()
    {
        if (currentReviewSegments.Count > 1)
        {
            currentReviewSegments.RemoveAt(currentReviewSegments.Count - 1);
        }
    }

    public void AddSegmentInReview(RecordSegment recSeg)
    {
        currentReviewSegments.Add(recSeg);
    }

}
