using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityProductionItem : MonoBehaviour
{
    public BuildingBlueprint BuildingBlueprint { get; set; }
    public UnitBlueprint UnitBlueprint { get; set; }
    public City MyCity { get; set; }
    public CityScreen MyCityScreen { get; set; }

    public BuildingJob thisBuildingJob;

    public bool IsUnit = false;

    public void OnClick()
    {
        //todo - get current city production

        if(thisBuildingJob == null)
        {
            BuildingJob buildingJob = null;
            if (BuildingBlueprint != null)
            {
                buildingJob = new BuildingJob(
                    BuildingBlueprint.name,
                    BuildingBlueprint.productionCost,
                    0,
                    () => {
                        BuildingBlueprint.OnBuildingComplete(MyCity);
                        Destroy(gameObject);
                    },
                    null
                );
            }
            else if(UnitBlueprint != null)
            {
                buildingJob = new BuildingJob(
                    UnitBlueprint.name,
                    UnitBlueprint.productionCost,
                    0,
                    () => {
                        UnitBlueprint.OnUnitComplete(MyCity);
                        MyCityScreen.RemoveCompletedUnitFromInProgress(UnitBlueprint, MyCity);
                    },
                    null
                    );
            }

            thisBuildingJob = buildingJob;
        }


        MyCity.SetCurrentBuildingJob(thisBuildingJob);

        if (MyCityScreen)
        {
            MyCityScreen.UpdateCurrentProductionText();
        }
    }
}
