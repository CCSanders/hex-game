using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    MouseController mouseController;
    public HexGrid grid;

    private void Start()
    {
        mouseController = GetComponent<MouseController>();
    }

    public delegate void OnUnitSelectionChanged(Unit unit);
    public event OnUnitSelectionChanged OnUnitChanged;

    Unit selectedUnit;
    public Unit SelectedUnit
    {
        get
        {
            return selectedUnit;
        }
        set
        {
            if (selectedUnit)
            {
                //clean up stuff related to unselecting the old unit
                selectedUnit.Location.DisableHighlight();
                selectedUnit.GetUnitView().ClearPath();
            }


            selectedUnit = value;

            if (selectedUnit)
            {
                //if the new unit isn't null, do stuff relating to selecting a unit.
                selectedUnit.Location.EnableHighlight(Color.blue);
                selectedUnit.GetUnitView().ShowPath();
            }

            OnUnitChanged?.Invoke(selectedUnit);
        }
    }
    
    public void DoSelection()
    {
        SelectedUnit = null;
        grid.ClearPath();

        HexCell currentCell = mouseController.GetCurrentCell();

        if (currentCell)
        {
            Unit[] currentUnits = currentCell.GetUnits();
            if (currentUnits != null && currentUnits.Length > 0)
            {
                //for now, only select the unit at index 0, ui can fix this later.
                SelectedUnit = currentUnits[0];
            }
        }
    }

    //City selectedCity = null

    public void SelectNextUnit(bool skipDoneUnits)
    {
        //get the current player

        List<Unit> units = grid.GetUnits();

        int currentIndex = 0;

        //find the current unit's index in the unit list
        if (SelectedUnit)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if(SelectedUnit == units[i])
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        for(int i = 0; i < units.Count; i++)
        {
            int tryIndex = (currentIndex + i + 1) % units.Count;

            if(skipDoneUnits && units[tryIndex].UnitWaitingForOrders() == false)
            {
                //if we don't care about done units and the current unit is done, skip.
                continue;
            }

            //if we aren't skippinbg done units or we found one that's waiting for orders, select it
            SelectedUnit = units[tryIndex];
            FindObjectOfType<MapCamera>()?.MoveToPosition(SelectedUnit.transform.position);
            return;
        }

        //if we skipped every other unit except the current selected one
        if(SelectedUnit.UnitWaitingForOrders() == false && skipDoneUnits)
        {
            SelectedUnit = null;
        }
    }
}
