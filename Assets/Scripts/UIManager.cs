using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Serializable]
    public struct ResourceFieldAttrib
    {
        public ResourceType resource;
        public Text text;
    }

    [SerializeField] private Pawn m_Pawn;
    
    [SerializeField] private ResourceFieldAttrib[] m_ResourceFieldAttrib;
    private Dictionary<ResourceType, Text> m_ResourceTextDict;

    void Awake()
    {
        
    }

    void Start()
    {
        m_ResourceTextDict = new Dictionary<ResourceType, Text>();
        
        foreach (var item in m_ResourceFieldAttrib)
        {
            if(!m_ResourceTextDict.ContainsKey(item.resource))
            {
                m_ResourceTextDict[item.resource] = item.text;
            }
        }
        
        foreach (KeyValuePair<ResourceType, int> resource in m_Pawn.Resources)
        {
            if(!m_ResourceTextDict.ContainsKey(resource.Key)) continue;
            ResourceChangeCallback(resource.Key, resource.Value);
        }
        
        m_Pawn.ResorceChangeEvent += ResourceChangeCallback;
    }

    public void ResourceChangeCallback(ResourceType resource, int newValue)
    {
        string resourceText = resource.ToString() + ": " + newValue.ToString();
        m_ResourceTextDict[resource].text = resourceText;
    }
}
