using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanelUI : MonoBehaviour
{
    public Text nameText;
    public Text movementText;

    private Unit currentUnit;

    public GameObject CityBuildButton;

    private SelectionController selectionController;

    private void Start()
    {
        selectionController = FindObjectOfType<SelectionController>();
        if (selectionController)
        {
            selectionController.OnUnitChanged += OnUnitChanged;
        }
    }

    private void OnUnitChanged(Unit unit)
    {
        //unsubscribe from on unit moved event, and change the current unit
        if (currentUnit) { currentUnit.OnUnitMoved -= OnUnitMoved; }
        currentUnit = unit;

        //if the current unit exists, update the panel and subscribe to the on unit moved event.
        if (currentUnit)
        {
            nameText.text = "Unit Name: " + currentUnit.unitName;
            movementText.text = "Movement " + currentUnit.movementRemaining + " / " + currentUnit.movement;

            if (currentUnit.canBuildCities)
            {
                CityBuildButton.SetActive(true);
                if(currentUnit.Location.GetCityWithinMyCell() != null)
                {
                    CityBuildButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    CityBuildButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                CityBuildButton.SetActive(false);
            }

            currentUnit.OnUnitMoved += OnUnitMoved;
        }
        else
        {
            nameText.text = "No Unit Selected.";
            movementText.text = "Movement: N/A";
            CityBuildButton.SetActive(false);
        }
    }

    //is called when the current unit's on move event is called. path isn't really used. 
    private void OnUnitMoved(List<HexCell> path)
    {
        if (currentUnit)
        {
            movementText.text = "Movement " + currentUnit.movementRemaining + " / " + currentUnit.movement;

            if (currentUnit.canBuildCities)
            {
                CityBuildButton.SetActive(true);
                if (currentUnit.Location.GetCityWithinMyCell() != null)
                {
                    CityBuildButton.GetComponent<Button>().interactable = false;
                }
                else
                {
                    CityBuildButton.GetComponent<Button>().interactable = true;
                }
            }
        }
    }
}
