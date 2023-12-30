using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AICfg
{
    [SerializeField, Range(0, 1)]
    public float smartMoveProbability;
    [SerializeField]
    public int intelligenceLevel;
}


[CreateAssetMenu(fileName = "AIConfig", menuName = "ScriptableObjects/AIConfig", order = 1)]
public class AIConfig : ScriptableObject
{
    [SerializeField]
    public List<AICfg> aiConfigs;
}
