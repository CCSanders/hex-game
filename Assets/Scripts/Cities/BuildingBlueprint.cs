using System.Collections;
using System.Collections.Generic;

//blueprint for a possible building that a city can build
public class BuildingBlueprint
{
    public readonly int id;
    public string name;
    public int productionCost = 3;

    /*
     * Possible fields to add here for any and all blueprints
     * 
     * START ERA
     * BUILDING TYPE
     * TECH REQUIRED
     * TECH THAT MAKES THIS OBSOLETE
     * GOLD MAINTENANCE
     * HELP TEXT
     * DESCRIPTION TEXT
     * ART
     * FOOD CARIED OVER
     */

    private List<BuildingYieldChange> buildingYieldChanges;
    private bool doesThisChangeYields = false;

    public enum BUILDING_TYPE { BUILDING, GLOBAL_WONDER, NATIONAL_WONDER };
    public BUILDING_TYPE building_type = BUILDING_TYPE.BUILDING;

    public BuildingBlueprint(int id, string name)
    {
        this.id = id;
        this.name = name;

        buildingYieldChanges = new List<BuildingYieldChange>();
    }

    public void AddYieldChange(BuildingYieldChange yc)
    {
        if(yc == null)
        {
            return;
        }

        doesThisChangeYields = true;
        buildingYieldChanges.Add(yc);
    }

    public void OnBuildingComplete(City city)
    {
        if (doesThisChangeYields)
        {
            city.AdjustYields(buildingYieldChanges);
        }

        city.SetCurrentBuildingJob(null);
        city.AddCompletedBuilding(this.id);
    }
}
