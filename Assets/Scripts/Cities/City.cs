using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public string cityName = "New City";
    public int healthPoints;

    public int factionID = 0;

    public float productionPerTurn = 1f;
    public float foodPerTurn = 2f;
    public float goldPerTurn = 0f;
    public float sciencePerTurn = 0f;
    public float culturePerTurn = 0f;

    public int currentPopulation = 1;

    private List<HexCell> cellsWithinBorders;
    private int visionRange = 3;

    BuildingJob buildingJob;
    List<int> buildingsCompleted;

    public HexGrid Grid { get; set; }

    public HexCell Location {
        get
        {
            return location;
        }
        set
        {
            location = value;
            location.AddCity(this);

            buildingsCompleted = new List<int>();

            Grid.IncreaseVisibility(value, visionRange);
            transform.localPosition = value.Position;
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }
    HexCell location;

    public void DoTurn()
    {
        //do production
        if (buildingJob != null)
        {
            Debug.Log("City is doing work on building job: " + buildingJob.buildingJobName);
            float workLeft = buildingJob.DoTurn(productionPerTurn);
            //todo: save overflow
        }

        //grow citizens

        //expand borders
    }

    public void AdjustYields(List<BuildingYieldChange> yieldChanges)
    {
        foreach(BuildingYieldChange yieldChange in yieldChanges)
        {
            switch (yieldChange.yieldType)
            {
                case BuildingYieldChange.YieldType.YIELD_FOOD:
                    foodPerTurn += yieldChange.byHowMuch;
                    break;
                case BuildingYieldChange.YieldType.YIELD_FOOD_PERCENT:
                    foodPerTurn *= (1f + yieldChange.byHowMuch); //eg. if the yield is an increase of 25%, it should be 1 + .25f = 1.25f
                    break;
                case BuildingYieldChange.YieldType.YIELD_CULTURE:
                    culturePerTurn += yieldChange.byHowMuch;
                    break;
                case BuildingYieldChange.YieldType.YIELD_CULTURE_PERCENT:
                    culturePerTurn *= (1f + yieldChange.byHowMuch);
                    break;
                case BuildingYieldChange.YieldType.YIELD_GOLD:
                    goldPerTurn += yieldChange.byHowMuch;
                    break;
                case BuildingYieldChange.YieldType.YIELD_GOLD_PERCENT:
                    goldPerTurn *= (1f + yieldChange.byHowMuch);
                    break;
                case BuildingYieldChange.YieldType.YIELD_SCIENCE:
                    sciencePerTurn += yieldChange.byHowMuch;
                    break;
                case BuildingYieldChange.YieldType.YIELD_SCIENCE_PERCENT:
                    sciencePerTurn *= (1f + yieldChange.byHowMuch);
                    break;
                case BuildingYieldChange.YieldType.YIELD_PRODUCTION:
                    productionPerTurn += yieldChange.byHowMuch;
                    break;
                case BuildingYieldChange.YieldType.YIELD_PRODUCTION_PERCENT:
                    productionPerTurn *= (1f + yieldChange.byHowMuch);
                    break;
            }
        }
    }

    public BuildingBlueprint[] GetPossibleBuildings()
    {
        //TODO: apply tech / uniqueness filters

        //current will return all buildings as long as the building hasn't already been completed
        return BuildingDatabase.GetListOfBuildingsWithBlacklist(buildingsCompleted);
    }

    public UnitBlueprint[] GetPossibleUnits()
    {
        return UnitDatabase.GetListOfUnits();
    }

    public BuildingJob GetCurrentBuildingJob()
    {
        return buildingJob;
    }

    public List<int> GetCompletedBuildings()
    {
        return buildingsCompleted;
    }

    public void SetCurrentBuildingJob(BuildingJob buildingJob)
    {
        this.buildingJob = buildingJob;
    }

    public void AddCompletedBuilding(int buildingID)
    {
        buildingsCompleted.Add(buildingID);
    }

    public void AddCellToTerritory(HexCell cell)
    {
        if(cellsWithinBorders == null)
        {
            cellsWithinBorders = new List<HexCell>();
        }
        cellsWithinBorders.Add(cell);
    }
}
