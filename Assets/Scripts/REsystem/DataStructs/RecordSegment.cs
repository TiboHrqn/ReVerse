using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class RecordSegment
{
    private static int numberOfSegment = 0;

    public int segmentNumber { get; private set; }

    public List<RecordSegment> nextSegments { get; private set; }

    public Dictionary<GameObject, GOInfos> goInfosDict { get; private set; }
    public Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>> compInfosDict { get; private set; }
    public List<TimeRecord> timeRecordList { get; private set; }

    public float firstTime { get; private set; }
    public float lastTime { get; private set; }
    public int nbRecord { get; private set; }

    public RecordSegment()
    {
        segmentNumber = numberOfSegment;
        numberOfSegment ++;
        nextSegments = new List<RecordSegment>();

        goInfosDict = new Dictionary<GameObject, GOInfos>();
        compInfosDict = new Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>>();
        timeRecordList = new List<TimeRecord>();

        Storer.recSegDict.Add(segmentNumber, this);
    }

    public RecordSegment(
        Dictionary<GameObject, GOInfos> goInfosDict,
        Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>> compInfosDict,
        List<TimeRecord> timeRecordList)
    {
        segmentNumber = numberOfSegment;
        numberOfSegment ++;
        nextSegments = new List<RecordSegment>();

        this.goInfosDict = goInfosDict ?? new Dictionary<GameObject, GOInfos>();
        this.compInfosDict = compInfosDict ?? new Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>>();
        this.timeRecordList = timeRecordList ?? new List<TimeRecord>();

        Storer.recSegDict.Add(segmentNumber, this);
    }

    public void AddNextSegment(RecordSegment recSeg)
    {
        nextSegments.Add(recSeg);
    }

    public void SetNextSegments(List<RecordSegment> recSegList)
    {
        nextSegments = recSegList;
    }

    public void ClearNextSegment()
    {
        nextSegments = new List<RecordSegment>();
    }


//------------------------------------------------------------------------------------
//------------------------------------ SPLIT PART ------------------------------------
//------------------------------------------------------------------------------------

public void Split(float time) //a optimiser (les listes de record sont triées par temps)
    {
        var beforeGoInfosDict = new Dictionary<GameObject, GOInfos>();
        var afterGoInfosDict = new Dictionary<GameObject, GOInfos>();

        var beforeCompInfosDict = new Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>>();
        var afterCompInfosDict = new Dictionary<GameObject, Dictionary<ComponentRecorder, List<ComponentRecord>>>();

        var beforeTimeRecordList = new List<TimeRecord>();
        var afterTimeRecordList = new List<TimeRecord>();

        //TIME RECORD PART
        foreach (TimeRecord timeRecord in timeRecordList)
        {
            if (timeRecord.time < time)
            {
                beforeTimeRecordList.Add(timeRecord);
            }
            else
            {
                afterTimeRecordList.Add(timeRecord);
            }
        }

        int maxTimeID = -1;
        if (beforeTimeRecordList.Count >=1)
        {
            maxTimeID = beforeTimeRecordList[beforeTimeRecordList.Count - 1].timeID;
        }

        //GO PART
        foreach (var kvp in goInfosDict)
        {
            List<GORecord> beforeGORecords = new List<GORecord>();
            List<GORecord> afterGORecords = new List<GORecord>();

            foreach (var record in kvp.Value.goRecords)
            {
                if (record.timeID <= maxTimeID)
                {
                    beforeGORecords.Add(record);
                }
                else
                {
                    afterGORecords.Add(record);
                }
            }

            afterGORecords.Insert(0 , beforeGORecords[beforeGORecords.Count - 1]);

            beforeGoInfosDict[kvp.Key] = new GOInfos(kvp.Value.goRecorder, beforeGORecords ); 
            afterGoInfosDict[kvp.Key] = new GOInfos(kvp.Value.goRecorder, afterGORecords);
        }

        //COMP PART
        foreach (var kvp in compInfosDict)
        {
            Dictionary<ComponentRecorder, List<ComponentRecord>> beforeComponentDict = new Dictionary<ComponentRecorder, List<ComponentRecord>>();
            Dictionary<ComponentRecorder, List<ComponentRecord>> afterComponentDict = new Dictionary<ComponentRecorder, List<ComponentRecord>>();
            foreach (var componentKvp in kvp.Value)
            {
                List<ComponentRecord> beforeComponentRecords = new List<ComponentRecord>();
                List<ComponentRecord> afterComponentRecords = new List<ComponentRecord>();

                foreach (ComponentRecord compRecord in componentKvp.Value)
                {
                    if (compRecord.timeID <= maxTimeID)
                    {
                        beforeComponentRecords.Add(compRecord);
                    }
                    else
                    {
                        afterComponentRecords.Add(compRecord);
                    }
                }

                afterComponentRecords.Insert(0 , beforeComponentRecords[beforeComponentRecords.Count - 1]);

                beforeComponentDict[componentKvp.Key] = beforeComponentRecords;
                afterComponentDict[componentKvp.Key] = afterComponentRecords;
                
            }

            beforeCompInfosDict[kvp.Key] = beforeComponentDict;
            afterCompInfosDict[kvp.Key] = afterComponentDict;
        }

        //construction du afterSegment
        RecordSegment afterSegment = new RecordSegment(
            afterGoInfosDict,
            afterCompInfosDict,
            afterTimeRecordList);
        afterSegment.SetNextSegments(nextSegments);

        //Modifs sur le segment actuel pour le faire devenir le "beforeSegment"
        goInfosDict = beforeGoInfosDict;
        compInfosDict = beforeCompInfosDict;
        timeRecordList = beforeTimeRecordList;
        ClearNextSegment();
        AddNextSegment(afterSegment);

        UpdateFirstLastTimeValue();
        afterSegment.UpdateFirstLastTimeValue();
        
        return;
    }

    


//-----------------------------------------------------------------------------------
//----------------------------------- RECORD PART -----------------------------------
//-----------------------------------------------------------------------------------

    public void AddTimeData(TimeRecord timeRecord)
    {
        timeRecordList.Add(timeRecord);
    }

    public void AddGORecord(GameObject go, GORecorder goRecorder, GORecord goRecord)
    {
        if (goInfosDict.ContainsKey(go))
        {
            goInfosDict[go].AddRecord(goRecord); 
        }
        else
        {
            goInfosDict.Add(go, new GOInfos(goRecorder, goRecord));
        }

    }    

    public void AddCompRecord(GameObject go, ComponentRecorder compRecorder, ComponentRecord compRecord)
    {
        if (compInfosDict.ContainsKey(go))
        {
            if (compInfosDict[go].ContainsKey(compRecorder))
            {
                compInfosDict[go][compRecorder].Add(compRecord);
            }
            else
            {
                compInfosDict[go].Add(compRecorder, new List<ComponentRecord>{compRecord});
            }
        }
        else
        {
            Dictionary<ComponentRecorder, List<ComponentRecord>> newDict = new Dictionary<ComponentRecorder, List<ComponentRecord>>();
            newDict.Add(compRecorder, new List<ComponentRecord>{compRecord});
            compInfosDict.Add(go, newDict);
        }
    }


//-----------------------------------------------------------------------------------
//----------------------------------- REVIEW PART -----------------------------------
//-----------------------------------------------------------------------------------

    public int GetPrevTimeID(float time)    //avec un binary search 
    {
        int left = 0;
        int right = nbRecord - 1;

        if (time < timeRecordList[left].time)
        {
            return timeRecordList[left].timeID;
        }

        if (time >= timeRecordList[right].time)
        {
            return timeRecordList[right].timeID;
        }

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (timeRecordList[mid].time == time)
            {
                return timeRecordList[mid].timeID;
            }
            else if (timeRecordList[mid].time < time)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return timeRecordList[right].timeID;
    }

    //fonctionne avec binary search
    //si l'id n'est pas dans la liste de temps, on renvoie le premier temps enregistré
    public float GetTimeFromID(int timeID) 
    {
        int left = 0;
        int right = nbRecord - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (timeRecordList[mid].timeID == timeID)
            {
                return timeRecordList[mid].time;
            }
            else if (timeRecordList[mid].timeID < timeID)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        //si le temps "n'existe pas", on renvoie la valeur du premier temps enregistré
        return timeRecordList[0].time;
    }

    public void UpdateFirstLastTimeValue()
    {
        if (timeRecordList != null)
        {
            firstTime = timeRecordList[0].time;
            lastTime = timeRecordList[timeRecordList.Count - 1].time;
            nbRecord = timeRecordList.Count;
        }
        else
        {
            throw new ArgumentException($"Aucun record");
        } 
    }

    public bool HasTimeBeenRecorded(float time)
    {
        if (time >= firstTime && time <= lastTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

      

    

    public void WriteSegmentTrace()   //ne marche pas
    {
        string strTrace = "{";

        strTrace += "\n\"FirstTime\":" + firstTime.ToString("F2", CultureInfo.InvariantCulture) + ",";
        strTrace += "\n\"LastTime\":" + lastTime.ToString("F2", CultureInfo.InvariantCulture) + ",";
        strTrace += "\n\"RecordDuration\":" + (lastTime - firstTime).ToString("F2", CultureInfo.InvariantCulture) + ",";

        strTrace += "\n\"TimeRecords\":" + JsonConvert.SerializeObject(timeRecordList);
        
        strTrace += ",\n\"GORecords\":[";
        
        foreach (var item in goInfosDict)
        {
            if (item.Key.tag == "WriteTrace")
            {
                strTrace += "\n{" + "\"GO\":\"" + item.Key.name + "\"" + ",\"Records\":";
                strTrace += JsonConvert.SerializeObject(item.Value.goRecords);
                strTrace += "},";
            }
        }
        strTrace += "],";

        strTrace += "\n\"CompRecords\": [";
        foreach (var item in compInfosDict)
        {
            if (item.Key.tag == "WriteTrace")
            {
                strTrace += "\n{" + "\"GO\": \"" + item.Key.name + "\"" + ", \"Records\": ";
                strTrace += "[";
                foreach (List<ComponentRecord> compRecords in item.Value.Values)
                {
                    strTrace += "[";
                    foreach (ComponentRecord compRecord in compRecords)
                    {
                        strTrace += compRecord.GetTrace() + ",";
                    }
                    strTrace += "{}]";
                }
                strTrace += "]";
                strTrace += "},{}";
            }
        }
        strTrace += "]\n}";
        File.WriteAllText("Trace/trace.json", strTrace);
    }
}

