using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingJob
{
    public string buildingJobName; //eg. "Petra"
    //public Image buildingJobIcon;

    public float totalProductionNeeded;
    public float productionDone;

    public delegate void ProductionCompleteDelegate();
    public event ProductionCompleteDelegate OnProductionComplete;

    public delegate float ProductionBonusDelegate();
    public ProductionBonusDelegate ProductionBonusFunc;

    public BuildingJob(
        string buildingJobName, 
        float totalProductionNeeded, 
        float overflowedProduction, 
        ProductionCompleteDelegate OnProductionComplete, 
        ProductionBonusDelegate ProductionBonusFunc
        )
    {
        if(OnProductionComplete == null)
        {
            throw new UnityException();
        }

        Debug.Log("new building job created");

        this.buildingJobName = buildingJobName;
        this.totalProductionNeeded = totalProductionNeeded;
        productionDone = overflowedProduction;
        this.OnProductionComplete = OnProductionComplete;
        this.ProductionBonusFunc = ProductionBonusFunc;
    }

    //returns the production left, complete if negative
    public float DoTurn(float cityProductionPerTurn)
    {
        productionDone += cityProductionPerTurn;

        if(productionDone >= totalProductionNeeded)
        {
            OnProductionComplete();
        }

        return totalProductionNeeded - productionDone;
    }

    public float GetWorkLeft()
    {
        return totalProductionNeeded - productionDone;
    }
}
