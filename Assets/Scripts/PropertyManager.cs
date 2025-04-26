using UnityEngine;

public class PropertyManager : MonoBehaviour
{
    // Get the logic of building properties in here. Check if player has the needed resources and create grid porperties in this class. Then just assign it to corresponding classes.
    // Don't forget the implementation for UI.
    
    [SerializeField] private BuildingData BuildingDataField;
    [SerializeField] static BuildingData buildingData;

    public static BuildingData BuildingData { get => buildingData; }

    void Start()
    {
        buildingData = BuildingDataField;
        buildingData.Init();
    }

    public static void ConstructBuilding(BuildingType type, Pawn pawn, int tileID)
    {
        // TO DO: Object Generator class (object pooling)
        GameObject buildingObject = Instantiate(buildingData.GetProperties(type).buildingModel);
        Building building = buildingObject.GetComponent<Building>();
        if(buildingObject)
        {
            Debug.Log("BuildingObject is not null");
        }
        
        building.Init(pawn, tileID);
    }
    
}
