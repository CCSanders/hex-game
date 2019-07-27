using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityScreen : MonoBehaviour
{
    City currentCity;

    public Text cityNameText;
    public GameObject statBlock;
    public GameObject buildingProductionListContent;
    public GameObject currentProductionPanel;
    public GameObject buildingsPanel;

    public GameObject CityProductionItemPrefab;
    public GameObject CompletedBuildingItemPrefab;

    private Dictionary<City, List<GameObject>> cityProdItemDict;
    private Dictionary<City, List<BuildingBlueprint>> cityBBAssignedDict;
    private Dictionary<City, List<UnitBlueprint>> cityUBAssignedDict;

    bool newProductionItemAdded = true;

    public void Open(City city)
    {
        Debug.Log("opening city ui");
        currentCity = city;

        if (!currentCity)
        {
            Debug.LogError("opened the city screen without a selected city");
            return;
        }

        if (cityProdItemDict == null)
        {
            cityProdItemDict = new Dictionary<City, List<GameObject>>();
        }

        if (cityBBAssignedDict == null)
        {
            cityBBAssignedDict = new Dictionary<City, List<BuildingBlueprint>>();
        }

        if(cityUBAssignedDict == null)
        {
            cityUBAssignedDict = new Dictionary<City, List<UnitBlueprint>>();
        }

        if (!cityProdItemDict.ContainsKey(currentCity))
        {
            cityProdItemDict.Add(currentCity, new List<GameObject>());
            newProductionItemAdded = true;
        }

        if (!cityBBAssignedDict.ContainsKey(currentCity))
        {
            cityBBAssignedDict.Add(currentCity, new List<BuildingBlueprint>());
        }

        if (!cityUBAssignedDict.ContainsKey(currentCity))
        {
            cityUBAssignedDict.Add(currentCity, new List<UnitBlueprint>());
        }

        //city name updated
        cityNameText.text = currentCity.cityName;

        //city stats updated
        UpdateCityStatsText();

        //building list populated
        if (newProductionItemAdded)
        {
            UpdateProductionItemList();
        }

        //current production job populated and math calculated
        UpdateCurrentProductionText();

        UpdateCompletedBuildings();
    }

    public void UpdateCurrentProductionText()
    {
        BuildingJob buildingJob = currentCity.GetCurrentBuildingJob();
        if (buildingJob == null)
        {
            currentProductionPanel.GetComponentInChildren<Text>().text = "No Current Project";
        }
        else
        {
            currentProductionPanel.GetComponentInChildren<Text>().text =
                buildingJob.buildingJobName + Environment.NewLine +
               "(" + (buildingJob.GetWorkLeft() / currentCity.productionPerTurn) + " Turns)" + Environment.NewLine +
               currentCity.productionPerTurn + " Production / Turn";
        }
    }

    public void UpdateCityStatsText()
    {
        Text statBlockText = statBlock.GetComponentInChildren<Text>();
        statBlockText.text = "Production: " + currentCity.productionPerTurn + Environment.NewLine;
        statBlockText.text += "Food: " + currentCity.foodPerTurn + Environment.NewLine;
        statBlockText.text += "Gold: " + currentCity.goldPerTurn + Environment.NewLine;
        statBlockText.text += "Science: " + currentCity.sciencePerTurn + Environment.NewLine;
        statBlockText.text += "Culture: " + currentCity.culturePerTurn + Environment.NewLine;
        statBlockText.text += "Current Population: " + currentCity.currentPopulation + Environment.NewLine;
    }

    public void UpdateProductionItemList()
    {
        BuildingBlueprint[] possibleBuildings = currentCity.GetPossibleBuildings();
        UnitBlueprint[] possibleUnits = currentCity.GetPossibleUnits();

        Debug.Log("size: " + cityProdItemDict[currentCity].Count);

        //reactivate buttons that have been created and stored
        foreach(GameObject item in cityProdItemDict[currentCity])
        {
            if(item == null)
            {
                continue;
            }

            item.SetActive(true);
        }

        //check for new buildings 
        foreach (BuildingBlueprint bb in possibleBuildings)
        {
            if (cityBBAssignedDict[currentCity].Contains(bb))
            {
                continue;
            }
            else
            {
                GameObject productionItem = Instantiate(CityProductionItemPrefab);
                productionItem.GetComponentInChildren<Text>().text = bb.name;
                productionItem.GetComponent<CityProductionItem>().BuildingBlueprint = bb;
                productionItem.GetComponent<CityProductionItem>().MyCity = currentCity;
                productionItem.GetComponent<CityProductionItem>().MyCityScreen = this;
                cityProdItemDict[currentCity].Add(productionItem);
                cityBBAssignedDict[currentCity].Add(bb);
                productionItem.transform.SetParent(buildingProductionListContent.transform);
            }
        }

        foreach (UnitBlueprint unit in possibleUnits)
        {
            if (cityUBAssignedDict[currentCity].Contains(unit))
            {
                continue;
            }
            else
            {
                GameObject productionItem = Instantiate(CityProductionItemPrefab);
                productionItem.GetComponentInChildren<Text>().text = unit.name;
                productionItem.GetComponent<CityProductionItem>().UnitBlueprint = unit;
                productionItem.GetComponent<CityProductionItem>().MyCity = currentCity;
                productionItem.GetComponent<CityProductionItem>().MyCityScreen = this;
                cityProdItemDict[currentCity].Add(productionItem);
                cityUBAssignedDict[currentCity].Add(unit);
                productionItem.transform.SetParent(buildingProductionListContent.transform);
            }
        }

        //newProductionItemAdded = false;
    }

    public void AddNewProductionItem()
    {
        //add
        newProductionItemAdded = true;
    }

    public void UpdateCompletedBuildings()
    {
        ClearCompletedBuildingsPanel();
        List<int> completedBuildingIDs = currentCity.GetCompletedBuildings();
        foreach(int id in completedBuildingIDs)
        {
            BuildingBlueprint building = BuildingDatabase.GetBuildingByID(id);
            if(building != null)
            {
                AddCompletedBuildingToPanel(building);
            }
        }
    }

    public void ClearCompletedBuildingsPanel()
    {
        //always skip the header
        for (int i = 1; i < buildingsPanel.transform.childCount; i++)
        {
            if (buildingsPanel.transform.GetChild(i))
            {
                Destroy(buildingsPanel.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ClearPossibleBuildingsPanel()
    {
        foreach(GameObject item in cityProdItemDict[currentCity])
        {
            if(item == null)
            {
                continue;
            }
            item.SetActive(false);
        }
    }

    public void AddCompletedBuildingToPanel(BuildingBlueprint completedBuilding)
    {
        GameObject completedBuildingItem = Instantiate(CompletedBuildingItemPrefab);
        completedBuildingItem.GetComponent<CompletedBuildingItem>().SetBuilding(completedBuilding);
        completedBuildingItem.transform.SetParent(buildingsPanel.transform);
    }

    public void RemoveCompletedUnitFromInProgress(UnitBlueprint unit, City city)
    {
        if(!city || unit == null || !cityProdItemDict.ContainsKey(city))
        {
            return;
        }

        GameObject unitToDestroy = null;

        if (cityUBAssignedDict[city].Contains(unit))
        {
            cityUBAssignedDict[city].Remove(unit);
        }
        else
        {
            return;
        }

        foreach(GameObject prodItem in cityProdItemDict[city])
        {
            if(prodItem != null && prodItem.GetComponent<CityProductionItem>().UnitBlueprint == unit)
            {
                cityProdItemDict[city].Remove(prodItem);
                unitToDestroy = prodItem;
                break;
            }
        }

        if(unitToDestroy != null)
        {
            cityProdItemDict[city].Remove(unitToDestroy);
            Destroy(unitToDestroy);
        }
    }

    public void OnClose()
    {
        //i want to destroy all children in the content pane and the completed buildings
        ClearCompletedBuildingsPanel();
        ClearPossibleBuildingsPanel();

        currentCity = null;
    }
}
