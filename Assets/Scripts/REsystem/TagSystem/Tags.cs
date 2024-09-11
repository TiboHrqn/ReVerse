using System;
using System.Collections.Generic;
using UnityEngine;


public class Tags : MonoBehaviour
{
    [SerializeField] private List<Tag> _allTags = new List<Tag>();
    public List<Tag> AllTags => _allTags;

    public bool HasTag(Tag t)
    {
        return _allTags.Contains(t);
    }

    public bool HasTag(string tagName)
    {
        return _allTags.Exists(t => t.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase));
    }

    public void TryHasTag(string tagName)
    {
        Debug.Log(HasTag(tagName));
    }

    public void AddTag(Tag t)
    {
        if (!_allTags.Contains(t))
        {
            _allTags.Add(t);
        }
    }

    public void RemoveTag(Tag t)
    {
        _allTags.Remove(t);
    }

    
}

public static class GameObjectExtensions
{
    public static bool HasTag(this GameObject go, Tag t)
    {
        return go.TryGetComponent<Tags>(out var tags) && tags.HasTag(t);
    }

    public static bool HasTag(this GameObject go, string t)
    {
        return go.TryGetComponent<Tags>(out var tags) && tags.HasTag(t);
    }

    public static void AddTag(this GameObject go, Tag t)
    {
        Tags tags = go.GetComponent<Tags>();
        if (tags == null)
        {
            tags = go.AddComponent<Tags>();
        }
        tags.AddTag(t);
    }

    public static void RemoveTag(this GameObject go, Tag t)
    {
        Tags tags = go.GetComponent<Tags>();
        if (tags != null)
        {
            tags.RemoveTag(t);
        }
        
    }


    
}
