using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TilePrefabAttrib
{
    public TileType TileType;
    public GameObject Prefab;
}

[Serializable]
public struct ResourcePrefabAttrib
{
    public ResourceType ResourceType;
    public GameObject Prefab;
}

[Serializable]
public struct MobPrefabAttrib
{
    public MobType MobType;
    public GameObject Prefab;
}

[CreateAssetMenu(menuName = "ScriptableObjects/GridObjectPrefabSettings", fileName = "PrefabSettings")]
public class GridObjectPrefabSettings : ScriptableObject
{
    [SerializeField] private List<TilePrefabAttrib> m_TilePrefabAttribs;
    [SerializeField] private List<ResourcePrefabAttrib> m_ResourcePrefabAttribs;
    [SerializeField] private List<MobPrefabAttrib> m_MobPrefabAttribs;
    
    private Dictionary<TileType, GameObject> m_TilePrefabDict;
    private Dictionary<ResourceType, GameObject> m_ResourcePrefabDict;
    private Dictionary<MobType, GameObject> m_MobPrefabDict;
    
    public void Init()
    {
        m_TilePrefabDict = new Dictionary<TileType, GameObject>();
        m_ResourcePrefabDict = new Dictionary<ResourceType, GameObject>();
        m_MobPrefabDict = new Dictionary<MobType, GameObject>();
    
        foreach (var attrib in m_TilePrefabAttribs)
        {
            if(!m_TilePrefabDict.ContainsKey(attrib.TileType))
            {
                m_TilePrefabDict.Add(attrib.TileType, attrib.Prefab);
            }
        }
        
        foreach (var attrib in m_ResourcePrefabAttribs)
        {
            if(!m_ResourcePrefabDict.ContainsKey(attrib.ResourceType))
            {
                m_ResourcePrefabDict.Add(attrib.ResourceType, attrib.Prefab);
            }
        }
        
        foreach (var attrib in m_MobPrefabAttribs)
        {
            if(!m_MobPrefabDict.ContainsKey(attrib.MobType))
            {
                m_MobPrefabDict.Add(attrib.MobType, attrib.Prefab);
            }
        }
    }
    
    public GameObject GetTilePrefab(TileType tileType)
    {
        return m_TilePrefabDict[tileType];
    }
    
    public GameObject GetResourcePrefab(ResourceType resourceType)
    {
        return m_ResourcePrefabDict[resourceType];
    }
    
    public GameObject GetMobPrefab(MobType mobType)
    {
        return m_MobPrefabDict[mobType];
    }
    
}
