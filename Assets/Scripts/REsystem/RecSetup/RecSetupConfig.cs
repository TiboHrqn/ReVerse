using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TypeNamePair
{
    [SerializeField] private string compRecName;
    [SerializeField] private string compName;
    public string CompRecName => compRecName;
    public string CompName => compName;
}

[CreateAssetMenu(menuName = "Scriptable Objects/RecSetupConfig")]
public class RecSetupConfig : ScriptableObject
{
    [SerializeField] private List<TypeNamePair> typeNamePair = new List<TypeNamePair>();
    public List<TypeNamePair> TypeNamePair => typeNamePair;
}
