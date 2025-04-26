using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingProperties
{
    public BuildingType buildingType;
    public GameObject buildingModel;
    public int food;
    public int production;
    public int science;
    public int gold;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Building Settings", fileName = "BuildingSettings")]
public class BuildingData : ScriptableObject
{   
    [SerializeField] public List<BuildingProperties> resourceReqsList;
    Dictionary<BuildingType, BuildingProperties> propertiesDict;
    
    public void Init()
    {
        propertiesDict = new Dictionary<BuildingType, BuildingProperties>();
        
        foreach (var requirement in resourceReqsList)
        {
            if(!propertiesDict.ContainsKey(requirement.buildingType))
            {
                propertiesDict[requirement.buildingType] = requirement;
                Debug.Log("See this" + propertiesDict[requirement.buildingType].buildingType);
            }
        }
    }
    
    public BuildingProperties GetProperties(BuildingType type)
    {
        return propertiesDict[type];
    }
    
}
