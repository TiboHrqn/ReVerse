using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EventType
    {
        Major,
        Minor
    }

public class REEvent
{
    public EventType eventType { get; private set; }
    public string title { get; private set; }
    public string description { get; private set; }
    public Color color { get; private set; }

    public REEvent(EventType eventType, string title, string description)
    {
        this.eventType = eventType;
        this.title  = title;
        this.description = description;
        color = Color.grey;
    }

    public REEvent(EventType eventType, string title, string description, Color color)
    {
        this.eventType = eventType;
        this.title  = title;
        this.description = description;
        this.color = color;
    }


}
